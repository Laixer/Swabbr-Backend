using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swabbr.Api.Authentication;
using Swabbr.Api.DataTransferObjects;
using Swabbr.Api.Helpers;
using Swabbr.Core.Interfaces.Services;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Transactions;

namespace Swabbr.Api.Controllers
{
    // FUTURE This will probably be changed up when we refactor the auth part.
    /// <summary>
    ///     Controller for authentication.
    /// </summary>
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
        /// <remarks>
        ///     FUTURE: The update call was removed here because of the complexity. Look into this.
        ///             Only the required user properties are set in the database.
        /// </remarks>
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] UserRegistrationDto input)
        {
            // Construct a new identity user for a new user based on the given input.
            // The entity will be created in our own data store.
            var identityUser = new SwabbrIdentityUser
            {
                Email = input.Email,
                Nickname = input.Nickname
            };

            // This call assigns the id to the identityUser object.
            var identityResult = await _userManager.CreateAsync(identityUser, input.Password);
            if (!identityResult.Succeeded)
            {
                return BadRequest("Could not create new user, contact your administrator");
            }

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
            var signInResult = await _signInManager.CheckPasswordSignInAsync(identityUser, input.Password, lockoutOnFailure: false);

            if (signInResult.Succeeded)
            {
                // Manage device registration
                var platform = input.PushNotificationPlatform;
                await _notificationService.RegisterAsync(identityUser.Id, platform, input.Handle);

                var tokenWrapper = _tokenService.GenerateToken(identityUser);

                // Map.
                var output = _mapper.Map<TokenWrapperDto>(tokenWrapper);

                // Return.
                return Ok(output);
            }

            if (signInResult.IsLockedOut)
            {
                return Unauthorized("Too many attempts.");
            }
            if (signInResult.IsNotAllowed)
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

            // Compose error message.
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
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> LogoutAsync()
        {
            // Act.
            await _signInManager.SignOutAsync();

            // Return.
            return NoContent();
        }
    }
}
