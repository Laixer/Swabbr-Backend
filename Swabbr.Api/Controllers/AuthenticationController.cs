﻿using Microsoft.AspNetCore.Authorization;
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
    [Route("authentication")]
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
                    this.Error(ErrorCodes.EntityAlreadyExists, "User already exists.")
                    );
            }

            //TODO: Add password strength check/constraints
            // TODO THOMAS The input should be 100% validated, never trust user input

            // Construct a new identity user for a new user based on the given input
            var identityUser = new SwabbrIdentityUser
            {
                // Generate new id for user to create
                // TODO THOMAS The database should handle this
                UserId = Guid.NewGuid(),

                // Use the input to set the properties
                Email = input.Email,
                FirstName = input.FirstName,
                LastName = input.LastName,
                BirthDate = input.BirthDate,
                Country = input.Country,
                Gender = (int)input.Gender, // TODO THOMAS Is this the way to go? Let's store in postgresql, which can handle enums. This is bug sensitive
                IsPrivate = input.IsPrivate,
                Nickname = input.Nickname,
                Timezone = input.Timezone,

                //TODO: Get URL from input, see uncommented line. Using URL for test purposes
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
                UserOutputModel userOutput = UserOutputModel.Parse(await _userRepository.GetAsync(identityUser.UserId));

                return Ok(
                    new UserAuthenticationOutputModel
                    {
                        Token = token,
                        Claims = await _userManager.GetClaimsAsync(identityUser),
                        Roles = await _userManager.GetRolesAsync(identityUser),
                        User = userOutput,
                        UserSettings = await _userSettingsRepository.GetForUserAsync(identityUser.UserId)
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
            // TODO THOMAS Validate user input! Null checks, format checks, etc

            // Retrieve the identity user
            var identityUser = await _userManager.FindByEmailAsync(input.Email);

            if (identityUser == null)
            {
                return Unauthorized(
                    this.Error(ErrorCodes.LoginFailed, "Invalid credentials.")
                    );
            }

            // Attempt a sign in using the user-provided password input
            var result = await _signInManager.CheckPasswordSignInAsync(identityUser, input.Password, lockoutOnFailure: false);

            if (result.IsLockedOut)
            {
                return Unauthorized(
                    this.Error(ErrorCodes.LoginFailed, "Too many attempts.")
                    );
            }
            if (result.IsNotAllowed)
            {
                return Unauthorized(
                    this.Error(ErrorCodes.LoginFailed, "Not allowed to log in.")
                    );
            }

            if (result.Succeeded)
            {
                // Login succeeded, generate and return access token
                var token = _tokenService.GenerateToken(identityUser);
                UserOutputModel userOutput = UserOutputModel.Parse(await _userRepository.GetAsync(identityUser.UserId));

                return Ok(new UserAuthenticationOutputModel
                {
                    Token = token,
                    Claims = await _userManager.GetClaimsAsync(identityUser),
                    Roles = await _userManager.GetRolesAsync(identityUser),
                    User = userOutput,
                    UserSettings = await _userSettingsRepository.GetForUserAsync(identityUser.UserId)
                });
            }

            // If we get here something definitely went wrong.
            return Unauthorized(
                this.Error(ErrorCodes.LoginFailed, "Could not log in.")
                );
        }

        //TODO: Refresh method

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