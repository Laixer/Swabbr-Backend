using Laixer.Utility.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swabbr.Api.Authentication;
using Swabbr.Core.Enums;
using Swabbr.Core.Interfaces.Services;
using System;
using System.Threading.Tasks;

namespace Swabbr.Api.Controllers
{

    /// <summary>
    /// Debug functionality.
    /// TODO Indicate this debug functionality for the final product!
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("debug")]
    public class DebugController : ApiControllerBase
    {

        private readonly IVlogTriggerService _vlogTriggerService;
        private readonly UserManager<SwabbrIdentityUser> _userManager;
        private readonly INotificationService _notificationService;
        private readonly ILivestreamPlaybackService _livestreamPlaybackService;
        private readonly IDeviceRegistrationService _deviceRegistrationService;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public DebugController(IVlogTriggerService vlogTriggerService,
            UserManager<SwabbrIdentityUser> userManager,
            INotificationService notificationService,
            ILivestreamPlaybackService livestreamPlaybackService,
            IDeviceRegistrationService deviceRegistrationService)
        {
            _vlogTriggerService = vlogTriggerService ?? throw new ArgumentNullException(nameof(vlogTriggerService));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _livestreamPlaybackService = livestreamPlaybackService ?? throw new ArgumentNullException(nameof(livestreamPlaybackService));
            _deviceRegistrationService = deviceRegistrationService ?? throw new ArgumentNullException(nameof(deviceRegistrationService));
        }

        /// <summary>
        /// Debug function to launch our <see cref="IVlogTriggerService"/>.
        /// </summary>
        /// <returns><see cref="OkResult"/></returns>
        [HttpPost("trigger_vlog")]
        public async Task<IActionResult> DriveVlogTriggerService(Guid userId)
        {
            try
            {
                userId.ThrowIfNullOrEmpty();

                //var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
                //if (user.Id != userId) { throw new InvalidOperationException("Not this user"); }

                await _vlogTriggerService.ProcessVlogTriggerForUserAsync(userId).ConfigureAwait(false);
                return Ok();
            }
            catch (Exception e)
            {
                return Conflict(e.Message); // TODO UNSAFE, change
            }
        }

        [HttpGet("fastly_token_test")]
        public async Task<IActionResult> FastlyTokenTest(Guid livestreamId)
        {
            try
            {
                livestreamId.ThrowIfNullOrEmpty();
                var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
                var token = await _livestreamPlaybackService.GetTokenAsync(livestreamId, user.Id).ConfigureAwait(false);
                return Ok(token);
            }
            catch (Exception e)
            {
                return Conflict(e.Message); // TODO UNSAFE, change
            }
        }

        [HttpPost("notification_test")]
        public async Task<IActionResult> NotificationTest(string message)
        {
            message.ThrowIfNullOrEmpty();
            var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
            await _notificationService.TestNotifationAsync(user.Id, message).ConfigureAwait(false);
            return Ok();
        }

        [HttpPost("register_device_for_user")]
        public async Task<IActionResult> RegisterDeviceForUser(Guid userId, string deviceHandle)
        {
            userId.ThrowIfNullOrEmpty();
            deviceHandle.ThrowIfNullOrEmpty();

            await _deviceRegistrationService.RegisterOnlyThisDeviceAsync(userId, PushNotificationPlatform.FCM, deviceHandle).ConfigureAwait(false);
            return Ok();
        }

    }
}
