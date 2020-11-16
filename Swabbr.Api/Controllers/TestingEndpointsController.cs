using Swabbr.Core.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Extensions.Logging;
using Swabbr.Api.Authentication;
using Swabbr.Api.Errors;
using Swabbr.Api.Extensions;
using Swabbr.Api.ViewModels.Enums;
using Swabbr.Core.Interfaces.Services;
using Swabbr.Core.Notifications;
using System;
using System.Threading.Tasks;

namespace Swabbr.Api.Controllers
{

    /// <summary>
    /// Contains testing endpoints.
    /// </summary>
    [ApiVersion("1")]
    [Route("api/{version:apiVersion}/testing_endpoints")]
    [Authorize]
    public sealed class TestingEndpointsController : ControllerBase
    {

        private readonly UserManager<SwabbrIdentityUser> _userManager;
        private readonly INotificationTestingService _notificationTestingService;
        private readonly ILogger logger;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public TestingEndpointsController(UserManager<SwabbrIdentityUser> userManager,
            INotificationTestingService notificationTestingService,
            ILoggerFactory loggerFactory)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _notificationTestingService = notificationTestingService ?? throw new ArgumentNullException(nameof(notificationTestingService));
            logger = (loggerFactory != null) ? loggerFactory.CreateLogger(nameof(TestingEndpointsController)) : throw new ArgumentNullException(nameof(loggerFactory));
        }

        /// <summary>
        /// Sends a notification to the currently logged in user.
        /// </summary>
        /// <remarks>
        /// This uses randomly generated values - they do not exist in the datastore.
        /// </remarks>
        /// <param name="actionAsString">The action as string value</param>
        /// <returns><see cref="OkObjectResult"/></returns>
        [HttpPost("request_test_notification/")]
        public async Task<IActionResult> RequestTestNotification(string actionAsString)
        {
            try
            {
                if (actionAsString.IsNullOrEmpty()) { return Conflict(this.Error(ErrorCodes.InvalidInput, "Specify the notification action")); }
                var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);

                switch (Parse(actionAsString))
                {
                    case NotificationActionModel.FollowedProfileLive:
                        await _notificationTestingService.NotifyFollowersProfileLiveAsync(user.Id).ConfigureAwait(false);
                        break;
                    case NotificationActionModel.FollowedProfileVlogPosted:
                        await _notificationTestingService.NotifyFollowersVlogPostedAsync(user.Id).ConfigureAwait(false);
                        break;
                    case NotificationActionModel.VlogGainedLikes:
                        await _notificationTestingService.NotifyVlogLikedAsync(user.Id).ConfigureAwait(false);
                        break;
                    case NotificationActionModel.VlogNewReaction:
                        await _notificationTestingService.NotifyReactionPlacedAsync(user.Id).ConfigureAwait(false);
                        break;
                    case NotificationActionModel.VlogRecordRequest:
                        await _notificationTestingService.NotifyVlogRecordRequestAsync(user.Id).ConfigureAwait(false);
                        break;
                    default:
                        return Conflict(this.Error(ErrorCodes.InvalidInput, "Given notification action is not available for testing"));
                }

                return Ok("Notification was sent");
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return Conflict(this.Error(ErrorCodes.InvalidOperation, "Could not send test notification"));
            }
        }

        private static NotificationActionModel Parse(string actionAsString)
        {
            if (actionAsString == NotificationActionTranslator.Translate(NotificationAction.FollowedProfileLive)) { return NotificationActionModel.FollowedProfileLive; }
            if (actionAsString == NotificationActionTranslator.Translate(NotificationAction.FollowedProfileVlogPosted)) { return NotificationActionModel.FollowedProfileVlogPosted; }
            if (actionAsString == NotificationActionTranslator.Translate(NotificationAction.InactiveUnwatchedVlogs)) { return NotificationActionModel.InactiveUnwatchedVlogs; }
            if (actionAsString == NotificationActionTranslator.Translate(NotificationAction.InactiveUserMotivate)) { return NotificationActionModel.InactiveUserMotivate; }
            if (actionAsString == NotificationActionTranslator.Translate(NotificationAction.InactiveVlogRecordRequest)) { return NotificationActionModel.InactiveVlogRecordRequest; }
            if (actionAsString == NotificationActionTranslator.Translate(NotificationAction.VlogGainedLikes)) { return NotificationActionModel.VlogGainedLikes; }
            if (actionAsString == NotificationActionTranslator.Translate(NotificationAction.VlogNewReaction)) { return NotificationActionModel.VlogNewReaction; }
            if (actionAsString == NotificationActionTranslator.Translate(NotificationAction.VlogRecordRequest)) { return NotificationActionModel.VlogRecordRequest; }
            throw new InvalidOperationException($"Could not parse {actionAsString}");
        }

    }

}
