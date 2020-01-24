using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swabbr.Api.Authentication;
using Swabbr.Api.Errors;
using Swabbr.Api.Extensions;
using Swabbr.Api.Services;
using Swabbr.Api.ViewModels;
using Swabbr.Core.Interfaces;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Swabbr.Api.Controllers
{
    /// <summary>
    /// Controller for handling requests related to authentication
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/v1/authentication")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserSettingsRepository _userSettingsRepository;
        private readonly ITokenService _tokenService;
        private readonly UserManager<SwabbrIdentityUser> _userManager;
        private readonly SignInManager<SwabbrIdentityUser> _signInManager;

        public AuthenticationController(
            IUserRepository userRepository,
            IUserSettingsRepository userSettingsRepository,
            ITokenService tokenService,
            UserManager<SwabbrIdentityUser> userManager,
            SignInManager<SwabbrIdentityUser> signInManager)
        {
            _userRepository = userRepository;
            _userSettingsRepository = userSettingsRepository;
            _tokenService = tokenService;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /// <summary>
        /// Create a new user account.
        /// </summary>
        /// <param name="input">Input for a new user to register</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("register")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(UserAuthenticationOutputModel))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(string))]
        public async Task<IActionResult> RegisterAsync([FromBody] UserRegisterInputModel input)
        {
            // Ensure the user does not already exist
            var userCheck = await _userManager.FindByEmailAsync(input.Email);

            if (userCheck != null)
            {
                return BadRequest(
                    this.Error(ErrorCodes.ENTITY_ALREADY_EXISTS, "User already exists.")
                    );
            }

            // TODO Password (strength?) check ...

            // Construct a new identity user for a new user based on the given input
            var identityUser = new SwabbrIdentityUser
            {
                // Generate new id for user to create
                UserId = Guid.NewGuid(),

                // Use the input to set the properties
                Email = input.Email,
                FirstName = input.FirstName,
                LastName = input.LastName,
                BirthDate = input.BirthDate,
                Country = input.Country,
                Gender = (int)input.Gender,
                IsPrivate = input.IsPrivate,
                Nickname = input.Nickname,
                Timezone = input.Timezone,

                //TODO Get URL from input, see uncommented line. Using URL for test purposes
                ////ProfileImageUrl = input.ProfileImageUrl,
                ProfileImageUrl = $"https://api.adorable.io/avatars/256/{input.Email}.png",
            };

            // The partition key and row key are both the UserId for now
            identityUser.PartitionKey = identityUser.UserId.ToString();
            identityUser.RowKey = identityUser.UserId.ToString();

            // Create the user
            var identityResult = await _userManager.CreateAsync(identityUser, input.Password);

            if (identityResult.Succeeded)
            {
                // Sign in the user that was just registered and return an access token
                await _signInManager.SignInAsync(identityUser, isPersistent: true);

                var token = _tokenService.GenerateToken(identityUser);
                UserOutputModel userOutput = await _userRepository.GetByIdAsync(identityUser.UserId);

                return Ok(
                    new UserAuthenticationOutputModel
                    {
                        Token = token,
                        Claims = await _userManager.GetClaimsAsync(identityUser),
                        Roles = await _userManager.GetRolesAsync(identityUser),
                        User = userOutput,
                        UserSettings = await _userSettingsRepository.GetByUserId(identityUser.UserId)
                    }
                );
            }

            // Something went wrong.
            return BadRequest();
        }

        /// <summary>
        /// Sign in an already registered user.
        /// </summary>
        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(UserAuthenticationOutputModel))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized, Type = typeof(string))]
        public async Task<IActionResult> LoginAsync([FromBody] UserAuthenticationInputModel input)
        {
            // Retrieve the identity user
            var identityUser = await _userManager.FindByEmailAsync(input.Email);

            if (identityUser == null)
            {
                return Unauthorized(
                    this.Error(ErrorCodes.LOGIN_FAILED, "Invalid credentials.")
                    );
            }

            // Attempt a sign in using the user-provided password input
            var result = await _signInManager.CheckPasswordSignInAsync(identityUser, input.Password, lockoutOnFailure: false);

            if (result.IsLockedOut)
            {
                return Unauthorized(
                    this.Error(ErrorCodes.LOGIN_FAILED, "Too many attempts.")
                    );
            }
            if (result.IsNotAllowed)
            {
                return Unauthorized(
                    this.Error(ErrorCodes.LOGIN_FAILED, "Not allowed to log in.")
                    );
            }

            if (result.Succeeded)
            {
                // Login succeeded, generate and return access token
                var token = _tokenService.GenerateToken(identityUser);
                UserOutputModel userOutput = await _userRepository.GetByIdAsync(identityUser.UserId);

                return Ok(new UserAuthenticationOutputModel
                {
                    Token = token,
                    Claims = await _userManager.GetClaimsAsync(identityUser),
                    Roles = await _userManager.GetRolesAsync(identityUser),
                    User = userOutput,
                    UserSettings = await _userSettingsRepository.GetByUserId(identityUser.UserId)
                });
            }

            // If we get here something definitely went wrong.
            return Unauthorized(
                this.Error(ErrorCodes.LOGIN_FAILED, "Could not log in.")
                );
        }

        /// <summary>
        /// Deauthorizes the authenticated user.
        /// </summary>
        [Authorize]
        [HttpDelete("logout")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> LogoutAsync()
        {
            await _signInManager.SignOutAsync();
            return NoContent();
        }
    }
}