using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swabbr.Api.Authentication;
using Swabbr.Core.Interfaces;
using Swabbr.Core.Notifications;
using System;
using System.Net;
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
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService service)
        {
            _notificationService = service;
        }

        /// <summary>
        /// Register a device to the notification hub.
        /// </summary>
        /// <returns></returns>
        [HttpPost("register")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(string))]
        public async Task<IActionResult> CreatePushRegistrationId()
        {
            var registrationId = await _notificationService.CreateRegistrationIdAsync();
            return Ok(registrationId);
        }

        /// <summary>
        /// Unregister a device from receiving push notifications.
        /// </summary>
        /// <param name="registrationId"></param>
        /// <returns></returns>
        [HttpDelete("unregister/{registrationId}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
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
        [HttpPut("enable/{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(string))]
        public async Task<IActionResult> RegisterForPushNotifications(string id, [FromBody] NotificationDeviceRegistration deviceUpdate)
        {
            //TODO Get user id and pass to method
            var userId = Guid.Parse(User.FindFirst(SwabbrClaimTypes.UserId).Value);

            // Create or update the given registration for push notifications
            NotificationResponse registrationResult = await _notificationService.RegisterUserForPushNotificationsAsync(id, userId, deviceUpdate);

            if (registrationResult.CompletedWithSuccess)
                return Ok();

            return BadRequest("An error occurred while sending push notification: " + registrationResult.FormattedErrorMessages);
        }

        /// <summary>
        /// Send push notification
        /// </summary>
        /// <param name="newNotification"></param>
        /// <returns></returns>
        //TODO: IMPORTANT! Make sure this method requires admin authorization. Temporarily disabled for testing purposes
        [AllowAnonymous]       
        //[Authorize(Roles = "Admin")]

        [HttpPost("send")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(string))]
        public async Task<IActionResult> SendNotification([FromBody] SwabbrNotification newNotification)
        {
            //TODO Pass user id in servic emethod
            NotificationResponse pushDeliveryResult = await _notificationService.SendNotificationToUserAsync(newNotification, new Guid("415adb6f-9573-49c1-83a5-ac72115a786f"));

            if (pushDeliveryResult.CompletedWithSuccess)
                return Ok();

            return BadRequest("An error occurred while sending push notification: " + pushDeliveryResult.FormattedErrorMessages);
        }
    }
}
