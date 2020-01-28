using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swabbr.Api.Authentication;
using Swabbr.Api.Errors;
using Swabbr.Api.Extensions;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Interfaces;
using Swabbr.Core.Notifications;
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
        private readonly INotificationRegistrationRepository _notificationRegistrationRepository;
        private readonly UserManager<SwabbrIdentityUser> _userManager;

        public NotificationsController(
            INotificationService notificationService,
            INotificationRegistrationRepository notificationRegistrationRepository,
            UserManager<SwabbrIdentityUser> userManager)
        {
            _notificationService = notificationService;
            _userManager = userManager;
            _notificationRegistrationRepository = notificationRegistrationRepository;
        }

        /// <summary>
        /// Unregister the authenticated user from receiving push notifications.
        /// </summary>
        /// <returns></returns>
        [HttpDelete("unregister")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> UnregisterFromNotificationsAsync()
        {
            var identityUser = await _userManager.GetUserAsync(User);

            try
            {
                var registration = await _notificationRegistrationRepository.GetByUserIdAsync(identityUser.UserId);

                // Delete the registration from the hub
                await _notificationService.DeleteRegistrationAsync(registration.RegistrationId);
                return Ok();
            }
            catch (EntityNotFoundException)
            {
                return BadRequest(
                    this.Error(ErrorCodes.ENTITY_NOT_FOUND, "Notification registration could not be found.")
                    );
            }
        }

        /// <summary>
        /// Register to receive push notifications
        /// </summary>
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

            return BadRequest(
                this.Error(ErrorCodes.EXTERNAL_ERROR, "Could not register device. " + registrationResult.FormattedErrorMessages)
                );
        }
    }
}