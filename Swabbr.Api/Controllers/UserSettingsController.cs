using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Swabbr.Api.Authentication;
using Swabbr.Api.Configuration;
using Swabbr.Api.ViewModels;
using Swabbr.Core.Entities;
using Swabbr.Core.Interfaces;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Swabbr.Api.Controllers
{

    /// <summary>
    /// Controller for handling requests related to any <see cref="UserSettings"/>.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("users/self/settings")]
    public class UserSettingsController : ControllerBase
    {

        private readonly IUserRepository _userRepository;
        private readonly UserManager<SwabbrIdentityUser> _userManager;
        private readonly UserSettingsConfiguration _userSettingsOptions;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public UserSettingsController(IUserRepository userRepository,
            UserManager<SwabbrIdentityUser> userManager,
            IOptions<UserSettingsConfiguration> userSettingsOptions)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            if (userSettingsOptions == null || userSettingsOptions.Value == null) { throw new ArgumentNullException(nameof(userSettingsOptions)); }
            _userSettingsOptions = userSettingsOptions.Value;
        }

        /// <summary>
        /// Get user settings for the authenticated user.
        /// </summary>
        /// <returns><see cref="OkResult"/></returns>
        [HttpGet("get")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(UserSettingsOutputModel))]
        public async Task<IActionResult> GetAsync()
        {
            var identityUser = await _userManager.GetUserAsync(User);
            if (identityUser == null) { throw new InvalidOperationException("Can't get user settings when no user is logged in"); }
            
            return Ok(await _userRepository.GetUserSettingsAsync(identityUser.Id));
        }

        /// <summary>
        /// Update user settings for the currently logged in user.
        /// </summary>
        /// <param name="input"><see cref="UserSettingsInputModel"/></param>
        /// <returns><see cref="OkResult"/> or <see cref="BadRequestResult"/></returns>
        [HttpPut("update")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(UserSettingsOutputModel))]
        public async Task<IActionResult> UpdateAsync([FromBody] UserSettingsInputModel input)
        {
            if (!ModelState.IsValid) { throw new InvalidOperationException("Input model is invalid"); }

            var identityUser = await _userManager.GetUserAsync(User);
            if (identityUser == null) { throw new InvalidOperationException("Can't modify user settings if no user is logged in"); }

            // Obtain settings and set updated properties.
            var settings = await _userRepository.GetUserSettingsAsync(identityUser.Id);
            settings.DailyVlogRequestLimit = input.DailyVlogRequestLimit;
            settings.FollowMode = input.FollowMode;
            settings.IsPrivate = input.IsPrivate;

            // Update and return (updated) settings entity
            return Ok(await _userRepository.UpdateUserSettingsAsync(settings));
        }

    }

}
