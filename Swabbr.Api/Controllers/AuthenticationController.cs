using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swabbr.Api.Authentication;
using Swabbr.Api.DataTransferObjects;
using Swabbr.Api.Helpers;
using Swabbr.Core.Helpers;
using Swabbr.Core.Interfaces.Services;
using System;
using System.Threading.Tasks;

namespace Swabbr.Api.Controllers
{
    // FUTURE: This will change when we refactor the auth part
    /// <summary>
    ///     Controller for authentication.
    /// </summary>
    /// <remarks>
    ///     We will refactor the authentication part and thus our user manager
    ///     in the future. Currently the user manager only handles the creation,
    ///     id assignment, login/logout and password management. All other user 
    ///     related operations are handled our core services/repos. 
    /// </remarks>
    [ApiController]
    [Route("authentication")]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserUpdateHelper _userUpdateHelper;
        private readonly TokenService _tokenService;
        private readonly UserManager<SwabbrIdentityUser> _userManager;
        private readonly SignInManager<SwabbrIdentityUser> _signInManager;
        private readonly INotificationService _notificationService;
        private readonly IMapper _mapper;

        /// <summary>
        ///     Create new instance.
        /// </summary>
        public AuthenticationController(UserUpdateHelper userUpdateHelper,
            TokenService tokenService,
            UserManager<SwabbrIdentityUser> userManager,
            SignInManager<SwabbrIdentityUser> signInManager,
            INotificationService notificationService,
            IMapper mapper)
        {
            _userUpdateHelper = userUpdateHelper ?? throw new ArgumentNullException(nameof(userUpdateHelper));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        // POST: api/authentication/register
        /// <summary>
        ///     Register a new user.
        /// </summary>
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] UserRegistrationDto input)
        {
            // TODO: The transactional part creates a bug. The created user doesn't 
            //       exist when the update call is made. A tempfix is removing the
            //       transaction scope. This means we can create the user but fail on
            //       updating the user. The call will return a failure but the user
            //       will exist after the call. This issue will no longer be relevant
            //       after the refactoring of the authentication part. See issue #217
            //       https://github.com/Laixer/Swabbr-Backend/issues/217.
            //
            //       Don't forget scope.Complete() when restoring!

            // Make this operation transactional.
            //using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            // Construct a new identity user for a new user based on the given input.
            // The entity will be created in our own data store.
            var identityUser = new SwabbrIdentityUser
            {
                Email = input.Email,
                Nickname = input.Nickname
            };

            // This call assigns the id to the identityUser object.
            // As mentioned, the user manager isn't used anywhere else.
            IdentityResult identityResult = await _userManager.CreateAsync(identityUser, input.Password);
            if (!identityResult.Succeeded)
            {
                return BadRequest("Could not create new user, contact your administrator");
            }

            // FUTURE: This will be removed when refactoring the auth part. 
            //         See the UserUpdateHelper for the current tempfix.
            // Assign all explicitly specified user properties
            // which are not handled by the user manager.
            await _userUpdateHelper.UpdateUserAsync(input, identityUser.Id);

            //scope.Complete();

            // Return.
            return NoContent();
        }

        // POST: api/authentication/login
        /// <summary>
        ///     Log a user in.
        /// </summary>
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] UserLoginDto input)
        {
            // Act.
            var identityUser = await _userManager.FindByEmailAsync(input.Email);
            if (identityUser is null)
            {
                // TODO Look at this
                return Unauthorized();
            }

            var signInResult = await _signInManager.CheckPasswordSignInAsync(identityUser, input.Password, lockoutOnFailure: false);

            if (signInResult.Succeeded)
            {
                // Manage device registration
                var platform = input.PushNotificationPlatform;
                await _notificationService.RegisterAsync(identityUser.Id, platform, input.Handle);

                var tokenWrapper = await _tokenService.GenerateTokenAsync(identityUser.Id);

                // Map.
                var output = _mapper.Map<TokenWrapperDto>(tokenWrapper);

                // Return.
                return Ok(output);
            }
            else if (signInResult.IsLockedOut)
            {
                return Unauthorized("User is locked out");
            }
            else if (signInResult.IsNotAllowed)
            {
                return Unauthorized("Not allowed to log in.");
            }

            // If we get here something definitely went wrong.
            return Unauthorized("Could not log in.");
        }

        // PUT: api/authentication/change-password
        /// <summary>
        ///     Change the current user password.
        /// </summary>
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePasswordAsync([FromBody] ChangePasswordDto input)
        {
            // Act.
            var identityUser = await _userManager.GetUserAsync(User);
            var signinResult = await _userManager.ChangePasswordAsync(identityUser, input.CurrentPassword, input.NewPassword);

            if (signinResult.Succeeded)
            {
                return NoContent();
            }

            var message = "Could not update password.";
            foreach (var error in signinResult.Errors)
            {
                message += $"\n\t{error.Description}";
            }

            // Return.
            return Conflict(message);
        }

        // POST: api/authentication/logout
        /// <summary>
        ///     Log the current user out.
        /// </summary>
        [HttpPost("logout")]
        public async Task<IActionResult> LogoutAsync([FromServices] Core.AppContext appContext)
        {
            // Act.
            await _notificationService.UnregisterAsync(appContext.UserId);
            await _tokenService.RevokeRefreshTokenAsync(appContext.UserId);
            await _signInManager.SignOutAsync();

            // Return.
            return NoContent();
        }

        // POST: api/authentication/refresh-token
        /// <summary>
        ///     Refresh an expired token using a refresh token.
        /// </summary>
        /// <remarks>
        ///     Using a JSON body with a POST is the current most elegant
        ///     solution for refreshing tokens, hence the design choice.
        /// </remarks>
        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshTokenRequestDto input)
        {
            // Act.
            var tokenWrapper = await _tokenService.RefreshTokenAsync(input.ExpiredToken, input.RefreshToken);

            // Map.
            var output = _mapper.Map<TokenWrapperDto>(tokenWrapper);

            // Return.
            return Ok(output);
        }
    }
}
