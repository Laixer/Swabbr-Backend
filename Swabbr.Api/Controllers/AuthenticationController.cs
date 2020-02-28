﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swabbr.Api.Authentication;
using Swabbr.Api.Errors;
using Swabbr.Api.Extensions;
using Swabbr.Api.Mapping;
using Swabbr.Api.Services;
using Swabbr.Api.ViewModels;
using Swabbr.Api.ViewModels.User;
using Swabbr.Core.Interfaces.Repositories;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Swabbr.Api.Controllers
{

    /// <summary>
    /// Controller for handling requests related to authentication.
    /// TODO Refresh method
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("authentication")]
    public class AuthenticationController : ControllerBase
    {

        private readonly IUserRepository _userRepository;
        private readonly IUserWithStatsRepository _userWithStatsRepository; // TODO Double functionality? Clean this up
        private readonly ITokenService _tokenService;
        private readonly UserManager<SwabbrIdentityUser> _userManager;
        private readonly SignInManager<SwabbrIdentityUser> _signInManager;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public AuthenticationController(IUserRepository userRepository,
            IUserWithStatsRepository userWithStatsRepository,
            ITokenService tokenService,
            UserManager<SwabbrIdentityUser> userManager,
            SignInManager<SwabbrIdentityUser> signInManager)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _userWithStatsRepository = userWithStatsRepository ?? throw new ArgumentNullException(nameof(userWithStatsRepository));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
        }

        /// <summary>
        /// Create a new user account.
        /// </summary>
        /// <remarks>
        /// TODO THOMAS The input should be 100% validated, never trust user input
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
            if (!ModelState.IsValid) { throw new InvalidOperationException("Input model is not valid"); }
            if ((await _userManager.FindByEmailAsync(input.Email).ConfigureAwait(false)) != null)
            {
                return BadRequest(this.Error(ErrorCodes.EntityAlreadyExists, "User already exists."));
            }

            // TODO This should not be possible if we are already logged in

            // Construct a new identity user for a new user based on the given input
            // TODO Make sure we copy all that we need here
            // TODO Check all properties here
            // TODO Shouldn't the mapper do this? Probably!
            var identityUser = new SwabbrIdentityUser
            {
                Email = input.Email,
                FirstName = input.FirstName,
                LastName = input.LastName,
                BirthDate = input.BirthDate,
                Country = input.Country,
                Gender = MapperEnum.Map(input.Gender),
                IsPrivate = input.IsPrivate,
                Nickname = input.Nickname,
                Timezone = input.Timezone,
                ProfileImageUrl = input.ProfileImageUrl,
            };

            // Create the user
            var identityResult = await _userManager.CreateAsync(identityUser, input.Password).ConfigureAwait(false);

            // Sign in and return
            if (identityResult.Succeeded)
            {
                // TODO This bugs out and can't sign in the first try. The exception says id is null, but the identityUser id gets set anyways!
                await _signInManager.SignInAsync(identityUser, isPersistent: true).ConfigureAwait(false);
                var token = _tokenService.GenerateToken(identityUser);
                var userOutput = MapperUser.Map(await _userWithStatsRepository.GetAsync(identityUser.Id).ConfigureAwait(false));

                return Ok(new UserAuthenticationOutputModel
                {
                    Token = token,
                    Claims = await _userManager.GetClaimsAsync(identityUser).ConfigureAwait(false),
                    Roles = await _userManager.GetRolesAsync(identityUser).ConfigureAwait(false),
                    User = userOutput,
                    UserSettings = MapperUser.Map(await _userRepository.GetUserSettingsAsync(identityUser.Id).ConfigureAwait(false)) // TODO User also contains settings, do we want this? How to handle?
                });
            }

            // Something went wrong
            return BadRequest();
        }

        /// <summary>
        /// Sign in an already registered user.
        /// </summary>
        /// <remarks>
        /// TODO THOMAS Validate user input! Null checks, format checks, etc
        /// </remarks>
        /// <param name="input"><see cref="UserAuthenticationInputModel"/></param>
        /// <returns><see cref="IActionResult"/></returns>
        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(UserAuthenticationOutputModel))]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized, Type = typeof(string))]
        public async Task<IActionResult> LoginAsync([FromBody] UserAuthenticationInputModel input)
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

                return Ok(new UserAuthenticationOutputModel
                {
                    Token = token,
                    Claims = await _userManager.GetClaimsAsync(identityUser).ConfigureAwait(false),
                    Roles = await _userManager.GetRolesAsync(identityUser).ConfigureAwait(false),
                    User = MapperUser.Map(await _userWithStatsRepository.GetAsync(identityUser.Id).ConfigureAwait(false)),
                    UserSettings = MapperUser.Map(await _userRepository.GetUserSettingsAsync(identityUser.Id).ConfigureAwait(false))
                });
            }

            // If we get here something definitely went wrong.
            return Unauthorized(this.Error(ErrorCodes.LoginFailed, "Could not log in."));
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
            // TODO What happens if we aren't signed in in the first place? --> BadRequest
            await _signInManager.SignOutAsync().ConfigureAwait(false);
            return NoContent();
        }

    }

}
