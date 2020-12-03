using Microsoft.Extensions.Logging;
using Swabbr.Core.Entities;
using Swabbr.Core.Enums;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Interfaces.Services;
using Swabbr.Core.Notifications;
using Swabbr.Core.Types;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Swabbr.Infrastructure.Notifications
{
    /// <summary>
    ///     Contains functionality to handle notification operations.
    /// </summary>
    /// <remarks>
    ///     This does no checks with regards to entity state whenever
    ///     a notification is invoked. Ensure that the external state
    ///     is correct before sending any notification.
    /// </remarks>
    public class NotificationService : INotificationService
    {
        protected readonly IUserRepository _userRepository;
        protected readonly NotificationClient _notificationClient;
        protected readonly INotificationRegistrationRepository _notificationRegistrationRepository;
        protected readonly ILogger<NotificationService> _logger;

        /// <summary>
        ///     Create new instance.
        /// </summary>
        public NotificationService(IUserRepository userRepository,
            NotificationClient notificationClient,
            INotificationRegistrationRepository notificationRegistrationRepository,
            ILogger<NotificationService> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _notificationClient = notificationClient ?? throw new ArgumentNullException(nameof(notificationClient));
            _notificationRegistrationRepository = notificationRegistrationRepository ?? throw new ArgumentNullException(nameof(notificationRegistrationRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        ///     Checks if the notification service is online.
        /// </summary>
        public virtual Task<bool> IsServiceOnlineAsync()
            => _notificationClient.IsServiceAvailableAsync();

        /// <summary>
        ///     Notify all followers of a user that a new vlog was posted.
        /// </summary>
        /// <param name="userId">User that posted a vlog.</param>
        /// <param name="vlogId">The posted vlog id.</param>
        public virtual async Task NotifyFollowersVlogPostedAsync(Guid userId, Guid vlogId)
        {
            _logger.LogTrace($"{nameof(NotifyFollowersVlogPostedAsync)} - Attempting notifying followers for posted vlog {vlogId} from user {userId}");

            var notification = NotificationBuilder.BuildFollowedProfileVlogPosted(vlogId, userId);

            // Notify each follower individually.
            await foreach (var item in _userRepository.GetFollowersPushDetailsAsync(userId, Navigation.All))
            {
                await _notificationClient.SendNotificationAsync(item.UserId, item.PushNotificationPlatform, notification);
                _logger.LogTrace($"{nameof(NotifyFollowersVlogPostedAsync)} - Notified user {item.UserId}");
            }

            _logger.LogTrace($"{nameof(NotifyFollowersVlogPostedAsync)} - Completed notifying followers for posted vlog {vlogId} from user {userId}");
        }

        /// <summary>
        ///     Send a vlog record request notification.
        /// </summary>
        /// <param name="userId">User id to notify.</param>
        /// <param name="vlogId">The suggested vlog id to post.</param>
        /// <param name="requestTimeout">The timeout time span for the request.</param>
        public virtual async Task NotifyVlogRecordRequestAsync(Guid userId, Guid vlogId, TimeSpan requestTimeout)
        {
            _logger.LogTrace($"{nameof(NotifyVlogRecordRequestAsync)} - Attempting vlog record request to user {userId}");

            // Log and return if we can't reach our user.
            if (!await _notificationRegistrationRepository.ExistsAsync(userId))
            {
                _logger.LogTrace($"Couldn't get notification registration for user {userId}");
                return;
            }

            var notification = NotificationBuilder.BuildRecordVlog(vlogId, DateTimeOffset.Now, requestTimeout);
            var pushDetails = await _userRepository.GetPushDetailsAsync(userId);
            await _notificationClient.SendNotificationAsync(pushDetails.UserId, pushDetails.PushNotificationPlatform, notification);

            _logger.LogTrace($"{nameof(NotifyVlogRecordRequestAsync)} - Completed vlog record request to user {userId}");
        }

        /// <summary>
        ///     Notify a user that a reaction was placed on one of 
        ///     the users own vlogs.
        /// </summary>
        /// <param name="receivingUserId">User that received the reaction.</param>
        /// <param name="vlogId">The id of the vlog.</param>
        /// <param name="reactionId">The placed reaction id.</param>
        public virtual async Task NotifyReactionPlacedAsync(Guid receivingUserId, Guid vlogId, Guid reactionId)
        {
            _logger.LogTrace($"{nameof(NotifyReactionPlacedAsync)} - Attempting vlog reaction notification for reaction {reactionId}");

            // Log and return if we can't reach our user.
            if (!await _notificationRegistrationRepository.ExistsAsync(receivingUserId))
            {
                _logger.LogTrace($"Couldn't get notification registration for user {receivingUserId}");
                return;
            }

            var userPushDetails = await _userRepository.GetPushDetailsAsync(receivingUserId);
            var notification = NotificationBuilder.BuildVlogNewReaction(vlogId, reactionId);
            await _notificationClient.SendNotificationAsync(userPushDetails.UserId, userPushDetails.PushNotificationPlatform, notification);

            _logger.LogTrace($"{nameof(NotifyReactionPlacedAsync)} - Attempting vlog reaction notification for reaction {reactionId}");
        }

        /// <summary>
        ///     Notify a user that one of the users vlogs received
        ///     a new like.
        /// </summary>
        /// <param name="receivingUserId">User that received the vlog like.</param>
        /// <param name="vlogLikeId">The vlog like id.</param>
        public virtual async Task NotifyVlogLikedAsync(Guid receivingUserId, VlogLikeId vlogLikeId)
        {
            if (vlogLikeId == null)
            {
                throw new ArgumentNullException(nameof(vlogLikeId));
            }

            _logger.LogTrace($"{nameof(NotifyVlogLikedAsync)} - Attempting vlog like notification for vlog like {vlogLikeId}");

            // Log and return if we can't reach our user.
            if (!await _notificationRegistrationRepository.ExistsAsync(receivingUserId))
            {
                _logger.LogTrace($"Couldn't get notification registration for user {receivingUserId}");
                return;
            }

            var userPushDetails = await _userRepository.GetPushDetailsAsync(receivingUserId);
            var notification = NotificationBuilder.BuildVlogGainedLike(vlogLikeId.VlogId, vlogLikeId.UserId);
            await _notificationClient.SendNotificationAsync(userPushDetails.UserId, userPushDetails.PushNotificationPlatform, notification);

            _logger.LogTrace($"{nameof(NotifyVlogLikedAsync)} - Attempting vlog like notification for vlog like {vlogLikeId}");
        }

        public virtual Task NotifyVlogRecordTimeoutAsync(Guid userId)
            => throw new NotImplementedException(nameof(NotifyVlogRecordTimeoutAsync));

        /// <summary>
        ///     Registers a device in Azure Notification Hub.
        /// </summary>
        /// <remarks>
        ///     This first unregisters all existing user registrations
        ///     for the specified <paramref name="userId"/>.
        /// </remarks>
        /// <param name="userId">The user to subscribe.</param>
        /// <param name="platform">The push notification platform.</param>
        /// <param name="handle">Device handle.</param>
        public virtual async Task RegisterAsync(Guid userId, PushNotificationPlatform platform, string handle)
        {
            // First clear the existing registration if it exists
            if (await _notificationRegistrationRepository.ExistsAsync(userId))
            {
                var currentRegistration = await _notificationRegistrationRepository.GetAsync(userId);
                await _notificationClient.UnregisterAsync(currentRegistration);
                await _notificationRegistrationRepository.DeleteAsync(currentRegistration.Id);
            }

            // Create new registration and assign external id
            var registration = await _notificationClient.RegisterAsync(new NotificationRegistration
            {
                Handle = handle,
                PushNotificationPlatform = platform,
                Id = userId
            });
            await _notificationRegistrationRepository.CreateAsync(registration);
        }

        public virtual Task UnregisterAsync(Guid userId)
            => throw new NotImplementedException();
    }
}
