using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.NotificationHubs;
using Swabbr.Core.Interfaces;
using Swabbr.Core.Notifications;
using System.Net;
using System.Threading.Tasks;

namespace Swabbr.Api.Controllers
{
    /// <summary>
    /// Controller for registering devices for notifications and sending out push notifications.
    /// </summary>
    
        // TODO Enable authorization?
    //[Authorize]
    [Route("api/v1/notifications")]
    public class NotificationsController : Controller
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService service)
        {
            _notificationService = service;
        }

        /// <summary>
        /// Get registration ID
        /// </summary>
        /// <returns></returns>
        [HttpGet("register")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(string))]
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
        public async Task<IActionResult> RegisterForPushNotifications(string id, [FromBody] DeviceRegistration deviceUpdate)
        {
            // Create or update the given registration for push notifications
            HubResponse registrationResult = await _notificationService.RegisterForPushNotificationsAsync(id, deviceUpdate);

            if (registrationResult.CompletedWithSuccess)
                return Ok();

            return BadRequest("An error occurred while sending push notification: " + registrationResult.FormattedErrorMessages);
        }

        /// <summary>
        /// Send push notification
        /// </summary>
        /// <param name="newNotification"></param>
        /// <returns></returns>
        
            //TODO: IMPORTANT! Make sure this method requires authorization. Temporarily disabled for testing purposes
        [AllowAnonymous]       
            //[Authorize(Roles = "Admin")]

        [HttpPost("send")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(string))]
        public async Task<IActionResult> SendNotification([FromBody] PushNotification newNotification)
        {
            HubResponse<NotificationOutcome> pushDeliveryResult = await _notificationService.SendNotificationAsync(newNotification);

            if (pushDeliveryResult.CompletedWithSuccess)
                return Ok();

            return BadRequest("An error occurred while sending push notification: " + pushDeliveryResult.FormattedErrorMessages);
        }
    }
}
