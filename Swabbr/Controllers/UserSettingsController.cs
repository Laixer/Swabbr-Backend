using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swabbr.Api.Authentication;
using Swabbr.Api.ViewModels;
using Swabbr.Core.Entities;
using Swabbr.Core.Enums;
using Swabbr.Core.Interfaces;
using System.Net;
using System.Threading.Tasks;

namespace Swabbr.Api.Controllers
{
    /// <summary>
    /// Controller for handling requests related to user settings.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/v1/users/self/settings")]
    public class UserSettingsController : ControllerBase
    {
        private readonly IUserSettingsRepository _userSettingsRepository;
        private readonly UserManager<SwabbrIdentityUser> _userManager;

        public UserSettingsController(IUserSettingsRepository userSettingsRepository, UserManager<SwabbrIdentityUser> userManager)
        {
            _userSettingsRepository = userSettingsRepository;
            _userManager = userManager;
        }

        /// <summary>
        /// Get user settings for the authenticated user.
        /// </summary>
        [HttpGet("get")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(UserSettingsOutputModel))]
        public async Task<IActionResult> GetAsync()
        {
            var identityUser = await _userManager.GetUserAsync(User);
            var userId = identityUser.UserId;
            var settingsEntity = await _userSettingsRepository.GetByUserId(userId);

            UserSettingsOutputModel output = settingsEntity;

            return Ok(output);
        }

        /// <summary>
        /// Update user settings.
        /// </summary>
        [HttpPut("update")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(UserSettingsOutputModel))]
        public async Task<IActionResult> UpdateAsync([FromBody] UserSettingsInputModel input)
        {
            var identityUser = await _userManager.GetUserAsync(User);

            // Ensure the user settings exist and belong to the authenticated user.
            UserSettings settings = await _userSettingsRepository.GetByUserId(identityUser.UserId);

            settings.DailyVlogRequestLimit = input.DailyVlogRequestLimit;
            settings.FollowMode = (FollowMode)((int)input.FollowMode);

            settings.IsPrivate = input.IsPrivate;

            await _userSettingsRepository.UpdateAsync(settings);

            // Return (updated) settings entity
            UserSettingsOutputModel output = settings;

            return Ok(output);
        }
    }
}