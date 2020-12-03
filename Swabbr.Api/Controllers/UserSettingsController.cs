using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swabbr.Api.Authentication;
using Swabbr.Api.Errors;
using Swabbr.Api.Extensions;
using Swabbr.Api.Mapping;
using Swabbr.Api.ViewModels;
using Swabbr.Core.Interfaces.Services;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Swabbr.Api.Controllers
{

    /// <summary>
    /// Controller for handling requests related to user settings.
    /// </summary>
    [Authorize]
    [ApiController]
    [ApiVersion("1")]
    [Route("api/{version:apiVersion}/users/self/settings")]
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
                // Act.
                var identityUser = await _userManager.GetUserAsync(User);
                var user = await _userService.GetAsync(identityUser.Id);

                // Map.
                var result = MapperUser.MapToSettings(user);

                // Return.
                return Ok(result);
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

                // Act.
                var identityUser = await _userManager.GetUserAsync(User);
                var user = await _userService.GetAsync(identityUser.Id);

                // Only assign properties which should be updated
                user.DailyVlogRequestLimit = input.DailyVlogRequestLimit;
                user.FollowMode = MapperEnum.Map(input.FollowMode);
                user.IsPrivate = input.IsPrivate;
                
                await _userService.UpdateAsync(user);

                // Map.
                var result = MapperUser.MapToSettings(await _userService.GetAsync(identityUser.Id));

                // Return.
                return Ok(result);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Could not update user settings");
                return Conflict(this.Error(ErrorCodes.InvalidOperation, "Could not update user settings"));
            }
        }

    }

}
