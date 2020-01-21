using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<SwabbrIdentityUser> _userManager;

        public NotificationsController(INotificationService service, UserManager<SwabbrIdentityUser> userManager)
        {
            _notificationService = service;
            _userManager = userManager;
        }

        /// <summary>
        /// Unregister a device from receiving push notifications.
        /// </summary>
        /// <param name="registrationId"></param>
        /// <returns></returns>
        [HttpDelete("unregister")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> UnregisterFromNotificationsAsync()
        {
            //TODO Get notification hub registration id for user
            var registrationId = "TODO";
            await _notificationService.DeleteRegistrationAsync(registrationId);
            return Ok();
        }

        /// <summary>
        /// Register to receive push notifications
        /// </summary>
        /// <param name="id"></param>
        /// <param name="deviceRegistration"></param>
        /// <returns></returns>
        [HttpPost("register")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(string))]
        public async Task<IActionResult> RegisterForPushNotificationsAsync([FromBody] DeviceRegistration deviceRegistration)
        {
            // Create a new registration
            var registrationId = await _notificationService.CreateRegistrationIdAsync();

            // Obtain the user Id
            var identityUser = await _userManager.GetUserAsync(User);
            var userId = identityUser.UserId;

            // Create or update the given registration for push notifications
            NotificationResponse registrationResult = await _notificationService.RegisterUserForPushNotificationsAsync(registrationId, userId, deviceRegistration);

            if (registrationResult.CompletedWithSuccess)
                return Ok();

            return BadRequest("Could not register device. " + registrationResult.FormattedErrorMessages);
        }

        /// <summary>
        /// Send push notification
        /// </summary>
        /// <param name="newNotification"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        //TODO: IMPORTANT! Make sure this method requires admin authorization. Temporarily disabled for testing purposes
        [AllowAnonymous]
        ////[Authorize(Roles = "Admin")]

        [HttpPost("send/{userId}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(string))]
        public async Task<IActionResult> SendNotificationAsync([FromBody] SwabbrNotification newNotification, Guid userId)
        {
            NotificationResponse pushDeliveryResult = await _notificationService.SendNotificationToUserAsync(newNotification, userId);

            if (pushDeliveryResult.CompletedWithSuccess)
            {
                return Ok("Notification was sent succesfully.");
            }

            return BadRequest("An error occurred while sending push notification: " + pushDeliveryResult.FormattedErrorMessages);
        }
    }
}