using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Swabbr.Api.Authentication;
using Swabbr.Api.Configuration;
using Swabbr.Api.Errors;
using Swabbr.Api.Extensions;
using Swabbr.Api.ViewModels;
using Swabbr.Core.Entities;
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
    [Route("users/self/settings")]
    public class UserSettingsController : ControllerBase
    {
        private readonly IUserSettingsRepository _userSettingsRepository;
        private readonly UserManager<SwabbrIdentityUser> _userManager;
        private readonly UserSettingsConfiguration _userSettingsOptions;

        public UserSettingsController(
            IUserSettingsRepository userSettingsRepository,
            UserManager<SwabbrIdentityUser> userManager,
            IOptions<UserSettingsConfiguration> userSettingsOptions
            )
        {
            _userSettingsRepository = userSettingsRepository;
            _userManager = userManager;
            _userSettingsOptions = userSettingsOptions.Value;
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

            // If no user settings exist for this user, create a new settings entity for the user
            // with default values.
            if (!(await _userSettingsRepository.ExistsForUserAsync(userId)))
            {
                await _userSettingsRepository.CreateAsync(new UserSettings
                {
                    UserId = userId
                });
            }

            //TODO: Custom key-value store property

            // Obtain and return the users' settings.
            UserSettingsOutputModel output = await _userSettingsRepository.GetForUserAsync(userId);
            return Ok(output);
        }

        /// <summary>
        /// Update user settings.
        /// </summary>
        [HttpPut("update")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(UserSettingsOutputModel))]
        public async Task<IActionResult> UpdateAsync([FromBody] UserSettingsInputModel input)
        {
            //TODO: Where to handle constraints like these?
            //TODO: Configuration values
            if (input.DailyVlogRequestLimit > _userSettingsOptions.DailyVlogRequestLimit)
            {
                return BadRequest(
                    this.Error(ErrorCodes.InvalidInput, "Input is invalid.")
                    );
            }

            var identityUser = await _userManager.GetUserAsync(User);
            var userId = identityUser.UserId;

            // If no user settings exist for this user, create a new settings entity for the user
            // with default values.
            if (!(await _userSettingsRepository.ExistsForUserAsync(userId)))
            {
                await _userSettingsRepository.CreateAsync(new UserSettings
                {
                    UserId = userId
                });
            }

            // Obtain settings and set updated properties.
            UserSettings settings = await _userSettingsRepository.GetForUserAsync(userId);

            settings.DailyVlogRequestLimit = input.DailyVlogRequestLimit;
            settings.FollowMode = input.FollowMode;
            settings.IsPrivate = input.IsPrivate;

            // Update and return (updated) settings entity
            UserSettingsOutputModel output = await _userSettingsRepository.UpdateAsync(settings);
            return Ok(output);
        }
    }
}