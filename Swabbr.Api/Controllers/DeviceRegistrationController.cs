using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swabbr.Api.Authentication;
using Swabbr.Api.Errors;
using Swabbr.Api.Extensions;
using Swabbr.Api.Mapping;
using Swabbr.Api.ViewModels.DeviceRegistration;
using Swabbr.Core.Interfaces.Services;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Swabbr.Api.Controllers
{

    /// <summary>
    /// Controller for registering devices. These endpoints should be called when
    /// a user logs in onto a device.
    /// </summary>
    [Authorize]
    [Route("device_registration")]
    public class DeviceRegistrationController : Controller
    {

        private readonly UserManager<SwabbrIdentityUser> _userManager;
        private readonly IDeviceRegistrationService _deviceRegistrationService;
        private readonly ILogger logger;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public DeviceRegistrationController() { }

        [HttpPost("register")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> RegisterForPushNotificationsAsync([FromBody] DeviceRegistrationInputModel model)
        {
            try
            {
                if (model == null) { return BadRequest(this.Error(ErrorCodes.InvalidInput, "Model can't be null")); }
                if (!ModelState.IsValid) { return BadRequest(this.Error(ErrorCodes.InvalidInput, "Model state is invalid")); }

                var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
                await _deviceRegistrationService.RegisterOnlyThisDeviceAsync(user.Id, MapperEnum.Map(model.Platform), model.Handle).ConfigureAwait(false);
                return Ok();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return Conflict(this.Error(ErrorCodes.InvalidOperation, "Could not register device"));
            }
        }

        [HttpDelete("unregister")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> UnregisterFromNotificationsAsync()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
                await _deviceRegistrationService.UnregisterAsync(user.Id).ConfigureAwait(false);
                return Ok();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return Conflict(this.Error(ErrorCodes.InvalidOperation, "Could not unregister device"));
            }
        }

    }

}
