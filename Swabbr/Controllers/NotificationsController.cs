using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.NotificationHubs;
using Swabbr.Core.Notifications;
using Swabbr.Infrastructure.Services;
using System.Threading.Tasks;

namespace Swabbr.Api.Controllers
{
    /// <summary>
    /// Controller for registering devices for notifications and sending out push notifications.
    /// </summary>
    [Authorize]
    [Route("api/v1/notifications")]
    public class NotificationsController : Controller
    {
        private readonly NotificationService _notificationService;

        public NotificationsController(NotificationService service)
        {
            _notificationService = service;
        }

        /// <summary>
        /// Get registration ID
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "User")]
        [HttpGet("register")]
        public async Task<IActionResult> CreatePushRegistrationId()
        {
            var registrationId = await _notificationService.CreateRegistrationIdAsync();
            return Ok(registrationId);
        }

        /// <summary>
        /// Delete registration ID and unregister from receiving push notifications
        /// </summary>
        /// <param name="registrationId"></param>
        /// <returns></returns>
        [Authorize(Roles = "User")]
        [HttpDelete("unregister/{registrationId}")]
        public async Task<IActionResult> UnregisterFromNotifications(string registrationId)
        {
            await _notificationService.DeleteRegistrationAsync(registrationId);
            return Ok();
        }

        /// <summary>
        /// Register to receive push notifications
        /// </summary>
        /// <param name="id"></param>
        /// <param name="deviceUpdate"></param>
        /// <returns></returns>
        [Authorize(Roles = "User")]
        [HttpPut("enable/{id}")]
        public async Task<IActionResult> RegisterForPushNotifications(string id, [FromBody] DeviceRegistration deviceUpdate)
        {
            HubResponse registrationResult = await _notificationService.RegisterForPushNotificationsAsync(id, deviceUpdate);

            if (registrationResult.CompletedWithSuccess)
                return Ok();

            return BadRequest("An error occurred while sending push notification: " + registrationResult.FormattedErrorMessages);
        }

        // TODO Implement in NHUB service
        /// <summary>
        /// Send push notification
        /// </summary>
        /// <param name="newNotification"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPost("send")]
        public async Task<IActionResult> SendNotification([FromBody] PushNotification newNotification)
        {
            HubResponse<NotificationOutcome> pushDeliveryResult = await _notificationService.SendNotificationAsync(newNotification);

            if (pushDeliveryResult.CompletedWithSuccess)
                return Ok();

            return BadRequest("An error occurred while sending push notification: " + pushDeliveryResult.FormattedErrorMessages);
        }
    }
}
