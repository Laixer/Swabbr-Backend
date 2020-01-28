using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swabbr.Api.Authentication;
using Swabbr.Api.Errors;
using Swabbr.Api.Extensions;
using Swabbr.Core.Interfaces;
using Swabbr.Core.Notifications;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Swabbr.Api.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("v1/api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly INotificationService _notificationService;
        private readonly UserManager<SwabbrIdentityUser> _userManager;

        public AdminController(
            IUserRepository userRepository,
            INotificationService notificationService,
            UserManager<SwabbrIdentityUser> userManager
            )
        {
            _userRepository = userRepository;
            _notificationService = notificationService;
            _userManager = userManager;
        }

        //TODO: Remove, temporary
        /// <summary>
        /// Used to remove tables to prevent unnecessary throughput billing
        /// </summary>
        /// <returns></returns>
        [Obsolete]
        [HttpDelete("deletestoragetables")]
        public async Task<IActionResult> TempDeleteTables()
        {
            await _userRepository.TempDeleteTables();
            return Ok();
        }

        /// <summary>
        /// Ban a specific user account.
        /// </summary>
        [Obsolete]
        [HttpPut("users/{userId}/ban")]
        public async Task<IActionResult> BanUserAsync()
        {
            //TODO Not implemented
            throw new NotImplementedException();
        }

        /// <summary>
        /// Delete a specific user account.
        /// </summary>
        [Obsolete]
        [HttpDelete("users/{userId}/delete")]
        public async Task<IActionResult> DeleteUserAsync()
        {
            //TODO Not implemented
            throw new NotImplementedException();
        }

        /// <summary>
        /// Send out a warning to a specific user.
        /// </summary>
        [Obsolete]
        [HttpPost("users/{userId}/warning")]
        public async Task<IActionResult> WarnUserAsync(string warningMessage)
        {
            //TODO Not implemented
            throw new NotImplementedException();
        }

        /// <summary>
        /// Send push notification
        /// </summary>
        /// <param name="newNotification"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        //TODO: IMPORTANT! Make sure this method requires admin authorization. Temporarily disabled for testing purposes
        [Authorize(Roles = "Admin")]
        [HttpPost("notifications/send/{userId}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(string))]
        public async Task<IActionResult> SendNotificationAsync([FromBody] SwabbrNotification newNotification, Guid userId)
        {
            NotificationResponse pushDeliveryResult = await _notificationService.SendNotificationToUserAsync(newNotification, userId);

            if (pushDeliveryResult.CompletedWithSuccess)
            {
                return Ok("Notification was sent succesfully.");
            }

            return BadRequest(
                this.Error(ErrorCodes.EXTERNAL_ERROR, "An error occurred while sending push notification: " + pushDeliveryResult.FormattedErrorMessages)
                );
        }
    }
}