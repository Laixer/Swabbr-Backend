using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swabbr.Api.Authentication;
using Swabbr.Api.Errors;
using Swabbr.Api.Extensions;
using Swabbr.Api.Parsing;
using Swabbr.Api.ViewModels.UserData;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace Swabbr.Api.Controllers
{

    /// <summary>
    /// Controller for handling user data, such as location and timezone.
    /// </summary>
    [Authorize]
    [ApiController]
    [ApiVersion("1")]
    [Route("api/{version:apiVersion}/userdata")]
    public sealed class UserDataController : ControllerBase
    {

        private readonly IUserRepository _userRepository;
        private readonly IUserWithStatsRepository _userWithStatsRepository;
        private readonly IUserService _userService;
        private readonly UserManager<SwabbrIdentityUser> _userManager;
        private readonly ILogger logger;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public UserDataController(IUserRepository userRepository,
            IUserWithStatsRepository userWithStatsRepository,
            IUserService userService,
            UserManager<SwabbrIdentityUser> userManager,
            ILoggerFactory loggerFactory)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _userWithStatsRepository = userWithStatsRepository ?? throw new ArgumentNullException(nameof(userWithStatsRepository));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            logger = (loggerFactory != null) ? loggerFactory.CreateLogger(nameof(UserDataController)) : throw new ArgumentNullException(nameof(loggerFactory));
        }

        /// <summary>
        /// Updates the user location.
        /// </summary>
        /// <param name="input"><see cref="UpdateLocationInputModel"/></param>
        /// <returns><see cref="OkResult"/></returns>
        [HttpPost("update_location")]
        public async Task<IActionResult> UpdateLocation([FromBody] UpdateLocationInputModel input)
        {
            try
            {
                if (input == null) { return BadRequest(this.Error(ErrorCodes.InvalidInput, "Post body is null")); }
                if (!ModelState.IsValid) { return BadRequest(this.Error(ErrorCodes.InvalidInput, "Post body is invalid")); }

                var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
                await _userService.UpdateLocationAsync(user.Id, input.Longitude, input.Latitude).ConfigureAwait(false);

                return Ok();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return Conflict(this.Error(ErrorCodes.InvalidOperation, "Could not update user location"));
            }
        }

        /// <summary>
        /// Updates the user timezone.
        /// </summary>
        /// <param name="input"><see cref="UpdateTimeZoneInputModel"/></param>
        /// <returns><see cref="OkResult"/></returns>
        [HttpPost("update_timezone")]
        public async Task<IActionResult> UpdateLocation([FromBody] UpdateTimeZoneInputModel input)
        {
            try
            {
                if (input == null) { return BadRequest(this.Error(ErrorCodes.InvalidInput, "Post body is null")); }
                if (!ModelState.IsValid) { return BadRequest(this.Error(ErrorCodes.InvalidInput, "Post body is invalid")); }

                var parsedTimeZone = TimeZoneInfoParser.Parse(input.Timezone);
                var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
                await _userService.UpdateTimeZoneAsync(user.Id, parsedTimeZone).ConfigureAwait(false);

                return Ok();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return Conflict(this.Error(ErrorCodes.InvalidOperation, "Could not update user timezone"));
            }
        }

    }

}
