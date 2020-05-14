using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents.SystemFunctions;
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
    [Route("authentication")]
    public class AuthenticationController : ControllerBase
    {

        private readonly IUserService _userService;
        private readonly IUserWithStatsRepository _userWithStatsRepository; // TODO Double functionality? Clean this up
        private readonly ITokenService _tokenService;
        private readonly UserManager<SwabbrIdentityUser> _userManager;
        private readonly SignInManager<SwabbrIdentityUser> _signInManager;
        private readonly IDeviceRegistrationService _deviceRegistrationService;
        private readonly ILogger logger;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public AuthenticationController(IUserService userService,
            IUserWithStatsRepository userWithStatsRepository,
            ITokenService tokenService,
            UserManager<SwabbrIdentityUser> userManager,
            SignInManager<SwabbrIdentityUser> signInManager,
            IDeviceRegistrationService deviceRegistrationService,
            ILoggerFactory loggerFactory)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _userWithStatsRepository = userWithStatsRepository ?? throw new ArgumentNullException(nameof(userWithStatsRepository));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            _deviceRegistrationService = deviceRegistrationService ?? throw new ArgumentNullException(nameof(deviceRegistrationService));
            logger = (loggerFactory != null) ? loggerFactory.CreateLogger(nameof(AuthenticationController)) : throw new ArgumentNullException(nameof(loggerFactory));
        }

        /// <summary>
        /// Create a new user account.
        /// </summary>
        /// <remarks>
        /// TODO THOMAS The input should be 100% validated, never trust user input.
        /// TODO This should not be possible if we are already logged in.
        /// </remarks>
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
                // Edge cases
                // TODO Re-enable
                //if (User.Identity.IsAuthenticated) { return BadRequest(this.Error(ErrorCodes.InvalidOperation, "User is already logged in")); }
                if ((await _userManager.FindByEmailAsync(input.Email).ConfigureAwait(false)) != null)
                {
                    return BadRequest(this.Error(ErrorCodes.EntityAlreadyExists, "User email already registered"));
                }

                // Construct a new identity user for a new user based on the given input
                // TODO Make this a service
                var identityUser = new SwabbrIdentityUser
                {
                    Email = input.Email,
                    Nickname = input.Nickname
                };

                var user = await _userManager.CreateAsync(identityUser, input.Password).ConfigureAwait(false);
                if (!user.Succeeded) { return BadRequest(this.Error(ErrorCodes.InvalidOperation, "Could not create new user, contact your administrator")); }

                // Update all other properties (if present)
                var updatedUser = await _userService.UpdateAsync(new UserUpdateWrapper
                {
                    UserId = identityUser.Id,
                    BirthDate = input.BirthDate,
                    Country = input.Country,
                    FirstName = input.FirstName,
                    Gender = MapperEnum.Map(input.Gender),
                    IsPrivate = input.IsPrivate,
                    LastName = input.LastName,
                    //Nickname = input.Nickname, Done in creation call
                    ProfileImageBase64Encoded = input.ProfileImageBase64Encoded
                }).ConfigureAwait(false);

                // Map and return
                return Ok(new UserAuthenticationOutputModel
                {
                    User = MapperUser.Map(updatedUser)
                });
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
                return Conflict(this.Error(ErrorCodes.InvalidOperation, "Something went wrong, contact your administrator for further assistance"));
            }
        }

        /// <summary>
        /// Sign in an already registered user.
        /// </summary>
        /// <remarks>
        /// TODO THOMAS Validate user input! Null checks, format checks, etc
        /// </remarks>
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

                // Throw a bad request if we are already logged in
                // TODO Implement
                // if (_signInManager.IsSignedIn()) { return BadRequest(this.Error(ErrorCodes.InvalidOperation, "Already logged in")); }

                var identityUser = await _userManager.FindByEmailAsync(input.Email).ConfigureAwait(false);
                if (identityUser == null) { return Unauthorized(this.Error(ErrorCodes.LoginFailed, "Invalid credentials")); }

                // Attempt a sign in using the user-provided password input
                var result = await _signInManager.CheckPasswordSignInAsync(identityUser, input.Password, lockoutOnFailure: false).ConfigureAwait(false);

                if (result.IsLockedOut) { return Unauthorized(this.Error(ErrorCodes.LoginFailed, "Too many attempts.")); }
                if (result.IsNotAllowed) { return Unauthorized(this.Error(ErrorCodes.LoginFailed, "Not allowed to log in.")); }
                if (result.Succeeded)
                {
                    // Login succeeded, generate and return access token
                    var token = _tokenService.GenerateToken(identityUser);

                    // Manage device registration
                    var pnp = (PushNotificationPlatformModel)input.PushNotificationPlatform;
                    await _deviceRegistrationService.RegisterOnlyThisDeviceAsync(identityUser.Id, MapperEnum.Map(pnp), input.Handle).ConfigureAwait(false);

                    return Ok(new UserAuthenticationOutputModel
                    {
                        Token = token,
                        Claims = await _userManager.GetClaimsAsync(identityUser).ConfigureAwait(false),
                        Roles = await _userManager.GetRolesAsync(identityUser).ConfigureAwait(false),
                        User = MapperUser.Map(await _userWithStatsRepository.GetAsync(identityUser.Id).ConfigureAwait(false)),
                        UserSettings = MapperUser.Map(await _userService.GetUserSettingsAsync(identityUser.Id).ConfigureAwait(false))
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
        /// <remarks>
        /// TODO Try Catch bulletproof
        /// </remarks>
        /// <returns><see cref="NoContentResult"/></returns>
        [Authorize]
        [HttpPost("logout")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> LogoutAsync()
        {
            // Unregister device
            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            await _deviceRegistrationService.UnregisterAsync(user.Id).ConfigureAwait(false);

            // TODO What happens if we aren't signed in in the first place? --> BadRequest
            await _signInManager.SignOutAsync().ConfigureAwait(false);
            return NoContent();
        }

    }

}
