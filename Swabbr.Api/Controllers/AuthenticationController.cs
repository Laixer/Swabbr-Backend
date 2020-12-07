using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swabbr.Api.Authentication;
using Swabbr.Api.DataTransferObjects;
using Swabbr.Core.Interfaces.Services;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Transactions;

namespace Swabbr.Api.Controllers
{
    /// <summary>
    ///     Controller for authentication.
    /// </summary>
    [Authorize]
    [Route("authentication")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly TokenService _tokenService;
        private readonly UserManager<SwabbrIdentityUser> _userManager;
        private readonly SignInManager<SwabbrIdentityUser> _signInManager;
        private readonly INotificationService _notificationService;
        private readonly IMapper _mapper;

        /// <summary>
        ///     Create new instance.
        /// </summary>
        public AuthenticationController(IUserService userService,
            TokenService tokenService,
            UserManager<SwabbrIdentityUser> userManager,
            SignInManager<SwabbrIdentityUser> signInManager,
            INotificationService notificationService,
            IMapper mapper)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] UserRegistrationDto input)
        {
            // Make this operation transactional
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            // Construct a new identity user for a new user based on the given input.
            // The entity will also be created in our own data store.
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

            // Update all other properties.
            // The nickname is handled by the creation call.
            var currentUser = await _userService.GetAsync(identityUser.Id);

            // TODO Duplicate code with user controller (update call)
            currentUser.BirthDate = input.BirthDate ?? currentUser.BirthDate;
            currentUser.Country = input.Country ?? currentUser.Country;
            currentUser.DailyVlogRequestLimit = input.DailyVlogRequestLimit ?? currentUser.DailyVlogRequestLimit;
            currentUser.FirstName = input.FirstName ?? currentUser.FirstName;
            currentUser.FollowMode = input.FollowMode ?? currentUser.FollowMode;
            currentUser.Gender = input.Gender ?? currentUser.Gender;
            currentUser.IsPrivate = input.IsPrivate ?? currentUser.IsPrivate;
            currentUser.LastName = input.LastName ?? currentUser.LastName;
            currentUser.Latitude = input.Latitude ?? currentUser.Latitude;
            currentUser.Longitude = input.Longitude ?? currentUser.Longitude;
            currentUser.Nickname = input.Nickname ?? currentUser.Nickname;
            currentUser.ProfileImageBase64Encoded = input.ProfileImageBase64Encoded ?? currentUser.ProfileImageBase64Encoded;
            currentUser.Timezone = input.Timezone ?? currentUser.Timezone;

            await _userService.UpdateAsync(currentUser);
            var updatedUser = await _userService.GetAsync(currentUser.Id);

            scope.Complete();

            // Map.
            var output = _mapper.Map<UserDto>(updatedUser);

            // Return.
            return Ok(output);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] UserLoginDto input)
        {
            // Act.
            var identityUser = await _userManager.FindByEmailAsync(input.Email);
            var signInResult = await _signInManager.CheckPasswordSignInAsync(identityUser, input.Password, lockoutOnFailure: false);

            if (signInResult.IsLockedOut)
            {
                return Unauthorized("Too many attempts.");
            }
            if (signInResult.IsNotAllowed)
            {
                return Unauthorized("Not allowed to log in.");
            }

            if (signInResult.Succeeded)
            {
                // Manage device registration
                var platform = input.PushNotificationPlatform;
                await _notificationService.RegisterAsync(identityUser.Id, platform, input.Handle);

                var tokenWrapper = _tokenService.GenerateToken(identityUser);

                // Map.
                var output = new SignedInDto
                {
                    CreateDate = tokenWrapper.CreateDate,
                    Token = tokenWrapper.Token,
                    TokenExpirationTimespan = tokenWrapper.TokenExpirationTimespan
                };

                // Return.
                return Ok(output);
            }

            // If we get here something definitely went wrong.
            return Unauthorized("Could not log in.");
        }

        [HttpPost("change_password")]
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
