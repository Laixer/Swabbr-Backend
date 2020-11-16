using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swabbr.Api.Authentication;
using Swabbr.Api.Errors;
using Swabbr.Api.Extensions;
using Swabbr.Api.Mapping;
using Swabbr.Api.Services;
using Swabbr.Api.ViewModels;
using Swabbr.Api.ViewModels.Enums;
using Swabbr.Api.ViewModels.User;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Interfaces.Services;
using Swabbr.Core.Types;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Transactions;

namespace Swabbr.Api.Controllers
{

    /// <summary>
    /// Controller for handling requests related to authentication.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/{version:apiVersion}/authentication")]
    public class AuthenticationController : ControllerBase
    {

        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        private readonly UserManager<SwabbrIdentityUser> _userManager;
        private readonly SignInManager<SwabbrIdentityUser> _signInManager;
        private readonly IDeviceRegistrationService _deviceRegistrationService;
        private readonly ILogger logger;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public AuthenticationController(IUserService userService,
            ITokenService tokenService,
            UserManager<SwabbrIdentityUser> userManager,
            SignInManager<SwabbrIdentityUser> signInManager,
            IDeviceRegistrationService deviceRegistrationService,
            ILoggerFactory loggerFactory)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            _deviceRegistrationService = deviceRegistrationService ?? throw new ArgumentNullException(nameof(deviceRegistrationService));
            logger = (loggerFactory != null) ? loggerFactory.CreateLogger(nameof(AuthenticationController)) : throw new ArgumentNullException(nameof(loggerFactory));
        }

        // TODO This does too much, maybe let the UserService handle a bunch.
        /// <summary>
        ///     Create a new user account.
        /// </summary>
        /// <param name="input"><see cref="UserRegisterInputModel"/></param>
        /// <returns><see cref="OkResult"/> or <see cref="BadRequestResult"/></returns>
        [AllowAnonymous]
        [HttpPost("register")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(UserAuthenticationOutputModel))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(string))]
        public async Task<IActionResult> RegisterAsync([FromBody] UserRegisterInputModel input)
        {
            if (input == null) { return BadRequest("Form body can't be null"); }
            if (!ModelState.IsValid) { return BadRequest("Input model is not valid"); }

            try
            {
                // Make this operation transactional
                using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

                // Edge cases
                if ((await _userManager.FindByEmailAsync(input.Email).ConfigureAwait(false)) != null)
                {
                    return BadRequest(this.Error(ErrorCodes.EntityAlreadyExists, "User email already registered"));
                }
                if (await _userService.ExistsNicknameAsync(input.Nickname).ConfigureAwait(false))
                {
                    return BadRequest(this.Error(ErrorCodes.EntityAlreadyExists, "Nickname already exists"));
                }

                // Construct a new identity user for a new user based on the given input.
                // The entity will also be created in our own data store.
                var identityUser = new SwabbrIdentityUser
                {
                    Email = input.Email,
                    Nickname = input.Nickname
                };

                // This call assigns the id to the identityUser object.
                var identityResult = await _userManager.CreateAsync(identityUser, input.Password).ConfigureAwait(false);
                if (!identityResult.Succeeded)
                {
                    return BadRequest(this.Error(ErrorCodes.InvalidOperation, "Could not create new user, contact your administrator"));
                }

                // Update all other properties.
                // The nickname is handled by the creation call.
                var user = await _userService.GetAsync(identityUser.Id).ConfigureAwait(false);
                
                user.BirthDate = input.BirthDate;
                user.Country = input.Country;
                user.FirstName = input.FirstName;
                user.Gender = MapperEnum.Map(input.Gender);
                user.IsPrivate = input.IsPrivate;
                user.LastName = input.LastName;
                user.ProfileImageBase64Encoded = input.ProfileImageBase64Encoded;

                var updatedUser = await _userService.UpdateAsync(user).ConfigureAwait(false);

                scope.Complete();

                // Map.
                var result = MapperUser.Map(updatedUser);

                // Return.
                return Ok(result);
            }
            catch (InvalidProfileImageStringException)
            {
                return BadRequest(this.Error(ErrorCodes.InvalidInput, "Profile image is invalid or not properly base64 encoded"));
            }
            catch (NicknameExistsException)
            {
                return Conflict(this.Error(ErrorCodes.EntityAlreadyExists, "Nickname is taken"));
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return Conflict(this.Error(ErrorCodes.InvalidOperation, "Something went wrong, contact your administrator for further assistance"));
            }
        }

        /// <summary>
        /// Sign in an already registered user.
        /// </summary>
        /// <param name="input"><see cref="UserLoginInputModel"/></param>
        /// <returns><see cref="IActionResult"/></returns>
        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(UserAuthenticationOutputModel))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized, Type = typeof(string))]
        public async Task<IActionResult> LoginAsync([FromBody] UserLoginInputModel input)
        {
            try
            {
                if (input == null) { return BadRequest("Form body can't be null"); }
                if (!ModelState.IsValid) { return BadRequest("Input model is not valid"); }

                var identityUser = await _userManager.FindByEmailAsync(input.Email).ConfigureAwait(false);
                if (identityUser == null) { return Unauthorized(this.Error(ErrorCodes.LoginFailed, "Invalid credentials")); }

                // Attempt a sign in using the user-provided password input
                var result = await _signInManager.CheckPasswordSignInAsync(identityUser, input.Password, lockoutOnFailure: false).ConfigureAwait(false);

                if (result.IsLockedOut) { return Unauthorized(this.Error(ErrorCodes.LoginFailed, "Too many attempts.")); }
                if (result.IsNotAllowed) { return Unauthorized(this.Error(ErrorCodes.LoginFailed, "Not allowed to log in.")); }
                if (result.Succeeded)
                {
                    // Login succeeded, generate and return access token
                    var tokenWrapper = _tokenService.GenerateToken(identityUser);

                    // Manage device registration
                    var pnp = (PushNotificationPlatformModel)input.PushNotificationPlatform;
                    await _deviceRegistrationService.RegisterOnlyThisDeviceAsync(identityUser.Id, MapperEnum.Map(pnp), input.Handle).ConfigureAwait(false);

                    return Ok(new UserAuthenticationOutputModel
                    {
                        Token = tokenWrapper.Token,
                        TokenCreationDate = tokenWrapper.CreateDate,
                        TokenExpirationTimespan = tokenWrapper.TokenExpirationTimespan,
                        Claims = await _userManager.GetClaimsAsync(identityUser).ConfigureAwait(false),
                        Roles = await _userManager.GetRolesAsync(identityUser).ConfigureAwait(false),
                        User = MapperUser.Map(await _userService.GetAsync(identityUser.Id).ConfigureAwait(false)),
                        UserSettings = MapperUser.MapToSettings(await _userService.GetAsync(identityUser.Id).ConfigureAwait(false))
                    });
                }

                // If we get here something definitely went wrong.
                return Unauthorized(this.Error(ErrorCodes.LoginFailed, "Could not log in."));
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return Conflict(this.Error(ErrorCodes.InvalidOperation, "Could not log in"));
            }
        }

        /// <summary>
        /// Used to update the user password.
        /// </summary>
        /// <param name="input"><see cref="UserChangePasswordInputModel"/></param>
        /// <returns><see cref="OkResult"/></returns>
        [HttpPost("change_password")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(UserAuthenticationOutputModel))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized, Type = typeof(string))]
        public async Task<IActionResult> ChangePasswordAsync([FromBody] UserChangePasswordInputModel input)
        {
            try
            {
                if (input == null) { return BadRequest("Form body can't be null"); }
                if (!ModelState.IsValid) { return BadRequest("Input model is not valid"); }

                var identityUser = await _userManager.GetUserAsync(User).ConfigureAwait(false);
                var result = await _userManager.ChangePasswordAsync(identityUser, input.CurrentPassword, input.NewPassword).ConfigureAwait(false);
                if (result.Succeeded)
                {
                    return Ok();
                }
                else
                {
                    var message = "Could not update password.";
                    foreach (var error in result.Errors)
                    {
                        message += $"\n\t{error.Description}";
                    }
                    return Conflict(this.Error(ErrorCodes.InvalidOperation, message));
                }
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return Conflict(this.Error(ErrorCodes.InvalidOperation, "Could not update password"));
            }
        }

        /// <summary>
        /// Deauthorizes the authenticated user.
        /// </summary>
        /// <returns><see cref="NoContentResult"/></returns>
        [Authorize]
        [HttpPost("logout")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> LogoutAsync()
        {
            try
            {
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    // Unregister device
                    var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
                    await _deviceRegistrationService.UnregisterAsync(user.Id).ConfigureAwait(false);
                    scope.Complete();
                }

                await _signInManager.SignOutAsync().ConfigureAwait(false);
                return NoContent();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return Conflict(this.Error(ErrorCodes.InvalidOperation, "Error while logging out"));
            }
        }

    }

}
