using Laixer.Utility.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swabbr.Api.Authentication;
using Swabbr.Api.Errors;
using Swabbr.Api.Extensions;
using Swabbr.Api.Mapping;
using Swabbr.Api.ViewModels;
using Swabbr.Core.Entities;
using Swabbr.Core.Interfaces.Services;
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

        private readonly IUserService _userService;
        private readonly UserManager<SwabbrIdentityUser> _userManager;
        private readonly ILogger logger;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public UserSettingsController(IUserService userService,
            UserManager<SwabbrIdentityUser> userManager,
            ILoggerFactory loggerFactory)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            logger = (loggerFactory != null) ? loggerFactory.CreateLogger(nameof(UserSettingsController)) : throw new ArgumentNullException(nameof(loggerFactory));
        }

        /// <summary>
        /// Get user settings for the authenticated user.
        /// </summary>
        /// <returns><see cref="OkResult"/></returns>
        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(UserSettingsOutputModel))]
        public async Task<IActionResult> GetAsync()
        {
            try
            {
                var identityUser = await _userManager.GetUserAsync(User).ConfigureAwait(false);
                if (identityUser == null) { throw new InvalidOperationException("Can't get user settings when no user is logged in"); }

                var result = await _userService.GetUserSettingsAsync(identityUser.Id).ConfigureAwait(false);
                return Ok(new UserSettingsOutputModel
                {
                    DailyVlogRequestLimit = result.DailyVlogRequestLimit,
                    FollowMode = result.FollowMode.GetEnumMemberAttribute(),
                    IsPrivate = result.IsPrivate,
                    UserId = result.UserId
                });
            }
            catch (Exception e)
            {
                logger.LogError(e, "Could not get user settings");
                return Conflict(this.Error(ErrorCodes.InvalidOperation, "Could not get user settings"));
            }
        }

        /// <summary>
        /// Update user settings for the currently logged in user.
        /// </summary>
        /// <param name="input"><see cref="UserSettingsInputModel"/></param>
        /// <returns><see cref="OkResult"/> or <see cref="BadRequestResult"/></returns>
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(UserSettingsOutputModel))]
        public async Task<IActionResult> UpdateAsync([FromBody] UserSettingsInputModel input)
        {
            try
            {
                if (input == null) { throw new ArgumentNullException("Model can't be null"); }
                if (!ModelState.IsValid) { throw new ArgumentException("Model isn't valid"); }

                var identityUser = await _userManager.GetUserAsync(User).ConfigureAwait(false);
                if (identityUser == null) { throw new InvalidOperationException("Can't update user settings when no user is logged in"); }

                var settings = new UserSettings
                {
                    DailyVlogRequestLimit = (int)input.DailyVlogRequestLimit,
                    FollowMode = MapperEnum.Map(input.FollowMode),
                    UserId = identityUser.Id,
                    IsPrivate = input.IsPrivate
                };
                await _userService.UpdateSettingsAsync(settings).ConfigureAwait(false);

                return Ok(MapperUser.Map(await _userService.GetUserSettingsAsync(identityUser.Id).ConfigureAwait(false)));
            }
            catch (Exception e)
            {
                logger.LogError(e, "Could not update user settings");
                return Conflict(this.Error(ErrorCodes.InvalidOperation, "Could not update user settings"));
            }
        }

    }

}
