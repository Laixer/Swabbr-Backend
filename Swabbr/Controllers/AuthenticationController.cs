using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swabbr.Api.Services;
using Swabbr.Api.ViewModels;
using Swabbr.Core.Entities;
using Swabbr.Core.Interfaces;
using Swabbr.Infrastructure.Data.Entities;
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
        private readonly UserManager<IdentityUserTableEntity> _userManager;
        private readonly SignInManager<IdentityUserTableEntity> _signInManager;

        public AuthenticationController(
            IUserRepository userRepository,
            ITokenService tokenService,
            UserManager<IdentityUserTableEntity> userManager,
            SignInManager<IdentityUserTableEntity> signInManager)
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
        [ProducesResponseType((int)HttpStatusCode.Created, Type = typeof(string))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Register([FromBody] UserRegisterInputModel input)
        {
            // Ensure the user does not already exist
            var userCheck = await _userManager.FindByEmailAsync(input.Email);

            if (userCheck != null)
            {
                return BadRequest("User already exists.");
            }
            // TODO Password check
            // ...

            // Convert input model to a user model
            User userFromInput = input;

            // Generate new id for user to create
            userFromInput.UserId = Guid.NewGuid();

            var createdUser = await _userRepository.CreateAsync(userFromInput);

            var identityUser = new IdentityUserTableEntity
            {
                UserId = createdUser.UserId,
                Email = createdUser.Email
            };

            var identityResult = await _userManager.CreateAsync(identityUser, input.Password);

            if (identityResult.Succeeded)
            {
                // Sign in the user that was just registered and return an access token
                await _signInManager.SignInAsync(identityUser, true);
                return Created(Url.ToString(), await _tokenService.GenerateToken(identityUser));
            }

            return BadRequest();
        }

        /// <summary>
        /// Sign in an already registered user.
        /// </summary>
        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(string))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> Login([FromBody] UserAuthenticateInputModel input)
        {
            var user = await _userManager.FindByEmailAsync(input.Email);

            if (user == null)
            {
                return Unauthorized("Invalid credentials.");
            }

            // Attempt a sign in using the user-provided password input
            var result = await _signInManager.PasswordSignInAsync(user, input.Password, input.RememberMe, false);

            if (result.IsLockedOut)
            {
                return Unauthorized("Too many attempts.");
            }
            if (result.IsNotAllowed)
            {
                return Unauthorized("Not allowed to log in.");
            }

            // Login succeeded, return access token
            if (result.Succeeded)
            {
                var token = await _tokenService.GenerateToken(user);
                return Ok(token);
            }

            // If we get here something went wrong.
            return Unauthorized();
        }

        /// <summary>
        /// Deauthorizes the authenticated user.
        /// </summary>
        [Authorize]
        [HttpDelete("logout")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return NoContent();
        }
    }
}
