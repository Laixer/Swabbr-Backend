using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swabbr.Api.Authentication;
using Swabbr.Api.Services;
using Swabbr.Api.ViewModels;
using Swabbr.Core.Entities;
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
        private readonly ITokenService _tokenService;
        private readonly UserManager<SwabbrIdentityUser> _userManager;
        private readonly SignInManager<SwabbrIdentityUser> _signInManager;

        public AuthenticationController(
            IUserRepository userRepository,
            ITokenService tokenService,
            UserManager<SwabbrIdentityUser> userManager,
            SignInManager<SwabbrIdentityUser> signInManager)
        {
            _userRepository = userRepository;
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
        public async Task<IActionResult> Register([FromBody] UserRegisterInputModel input)
        {
            // Ensure the user does not already exist
            var userCheck = await _userManager.FindByEmailAsync(input.Email);

            if (userCheck != null)
            {
                return BadRequest("User already exists.");
            }

            // TODO Password check ...

            // Convert input model to a user model
            User userFromInput = input;

            // Generate new id for user to create
            userFromInput.UserId = Guid.NewGuid();

            // Create the user
            var createdUser = await _userRepository.CreateAsync(userFromInput);

            // Construct a new identity user based on the created user entity
            var identityUser = new SwabbrIdentityUser
            {
                UserId = createdUser.UserId,
                Email = createdUser.Email,
                PartitionKey = createdUser.UserId.ToString(),
                RowKey = createdUser.UserId.ToString()
            };

            // Create the identity user
            var identityResult = await _userManager.CreateAsync(identityUser, input.Password);

            if (identityResult.Succeeded)
            {
                // Sign in the user that was just registered and return an access token
                await _signInManager.SignInAsync(identityUser, true);

                var token = _tokenService.GenerateToken(identityUser);
                UserOutputModel userOutput = await _userRepository.GetByIdAsync(identityUser.UserId);

                return Ok(
                    new UserAuthenticationOutputModel
                    {
                        Token = token,
                        Claims = await _userManager.GetClaimsAsync(identityUser),
                        Roles = await _userManager.GetRolesAsync(identityUser),
                        User = userOutput,
                        // TODO UserSettings (Must be created and initialized to default during registration)
                        UserSettings = MockData.MockRepository.RandomUserSettingsOutput()
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
        public async Task<IActionResult> Login([FromBody] UserAuthenticationInputModel input)
        {
            // Retrieve the identity user
            var identityUser = await _userManager.FindByEmailAsync(input.Email);

            if (identityUser == null)
            {
                return Unauthorized("Invalid credentials.");
            }

            // Attempt a sign in using the user-provided password input
            var result = await _signInManager.PasswordSignInAsync(identityUser, input.Password, input.RememberMe, false);

            if (result.IsLockedOut)
            {
                return Unauthorized("Too many attempts.");
            }
            if (result.IsNotAllowed)
            {
                return Unauthorized("Not allowed to log in.");
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
                    // TODO UserSettings in output model
                    UserSettings = MockData.MockRepository.RandomUserSettingsOutput()
                });
            }

            // If we get here something definitely went wrong.
            return Unauthorized();
        }

        /// <summary>
        /// Deauthorizes the authenticated user.
        /// </summary>
        [Authorize]
        [HttpDelete("logout")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return NoContent();
        }
    }
}