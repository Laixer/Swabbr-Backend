using Microsoft.Extensions.Logging;
using Swabbr.Core.BackgroundWork;
using Swabbr.Core.Entities;
using Swabbr.Core.Interfaces.Clients;
using Swabbr.Core.Interfaces.Factories;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Interfaces.Services;
using Swabbr.Core.Notifications.BackgroundTasks;
using Swabbr.Core.Notifications.Data;
using Swabbr.Core.Types;
using System;
using System.Threading.Tasks;

namespace Swabbr.Infrastructure.Notifications
{
    /// <summary>
    ///     Contains functionality to handle notification operations.
    ///     All methods in this service which send a notification are
    ///     dispatched to the dispatch manager. Each of these methods
    ///     returns immediately.
    /// </summary>
    /// <remarks>
    ///     This does no checks with regards to entity state whenever
    ///     a notification is invoked. Ensure that the external state
    ///     is correct before sending any notification.
    /// </remarks>
    public class NotificationService : INotificationService
    {
        private readonly INotificationClient _notificationClient;
        private readonly INotificationRegistrationRepository _notificationRegistrationRepository;
        private readonly DispatchManager _dispatchManager;
        private readonly IUserRepository _userRepository;
        private readonly INotificationFactory _notificationFactory;
        private readonly ILogger<NotificationService> _logger;

        /// <summary>
        ///     Create new instance.
        /// </summary>
        public NotificationService(INotificationClient notificationClient,
            INotificationRegistrationRepository notificationRegistrationRepository,
            DispatchManager dispatchManager,
            IUserRepository userRepository,
            INotificationFactory notificationFactory,
            ILogger<NotificationService> logger)
        {
            _notificationClient = notificationClient ?? throw new ArgumentNullException(nameof(notificationClient));
            _notificationRegistrationRepository = notificationRegistrationRepository ?? throw new ArgumentNullException(nameof(notificationRegistrationRepository));
            _dispatchManager = dispatchManager ?? throw new ArgumentNullException(nameof(dispatchManager));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _notificationFactory = notificationFactory ?? throw new ArgumentNullException(nameof(notificationFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        ///     Notify all followers of a user that a new vlog was posted.
        /// </summary>
        /// <remarks>
        ///     A notification background task is dispatched to the 
        ///     <see cref="DispatchManager"/> for each follower.
        /// </remarks>
        /// <param name="vlogOwnerUserId">Owner of the vlog.</param>
        /// <param name="vlogId">The posted vlog id.</param>
        public virtual async Task NotifyFollowersVlogPostedAsync(Guid vlogOwnerUserId, Guid vlogId)
        {
            _logger.LogTrace($"Notifying followers that {vlogOwnerUserId} posted vlog {vlogId}");

            await foreach (var user in _userRepository.GetFollowersAsync(vlogOwnerUserId, Navigation.All))
            {
                var notification = _notificationFactory.BuildFollowedProfileVlogPosted(user.Id, vlogId, vlogOwnerUserId);

                _logger.LogTrace($"Notifying follower {user.Id} that {vlogOwnerUserId} posted a vlog");

                _dispatchManager.Dispatch<NotifyBackgroundTask<DataFollowedProfileVlogPosted>>(notification);
            }
        }

        /// <summary>
        ///     Send a vlog record request notification.
        /// </summary>
        /// <remarks>
        ///     This is dispatched to the <see cref="DispatchManager"/>.
        /// </remarks>
        /// <param name="userId">User id to notify.</param>
        /// <param name="vlogId">The suggested vlog id to post.</param>
        /// <param name="requestTimeout">The timeout time span for the request.</param>
        public virtual Task NotifyVlogRecordRequestAsync(Guid userId, Guid vlogId, TimeSpan requestTimeout)
        {
            var notification = _notificationFactory.BuildVlogRecordRequest(userId, vlogId, DateTimeOffset.Now, requestTimeout);

            _logger.LogTrace($"Sending vlog record request to {userId}");

            _dispatchManager.Dispatch<NotifyBackgroundTask<DataVlogRecordRequest>>(notification);

            return Task.CompletedTask;
        }

        /// <summary>
        ///     Notify a user that a reaction was placed on one of 
        ///     the users own vlogs.
        /// </summary>
        /// <remarks>
        ///     This is dispatched to the <see cref="DispatchManager"/>.
        /// </remarks>
        /// <param name="receivingUserId">User that received the reaction.</param>
        /// <param name="vlogId">The id of the vlog.</param>
        /// <param name="reactionId">The placed reaction id.</param>
        public virtual Task NotifyReactionPlacedAsync(Guid receivingUserId, Guid vlogId, Guid reactionId)
        {
            var notificationContext = _notificationFactory.BuildVlogNewReaction(receivingUserId, vlogId, reactionId);

            _logger.LogTrace($"Notifying user {receivingUserId} that {vlogId} gained a reaction");

            _dispatchManager.Dispatch<NotifyBackgroundTask<DataVlogNewReaction>>(notificationContext);

            return Task.CompletedTask;
        }

        /// <summary>
        ///     Notify a user that one of the users vlogs received
        ///     a new like.
        /// </summary>
        /// <remarks>
        ///     This is dispatched to the <see cref="DispatchManager"/>.
        /// </remarks>
        /// <param name="receivingUserId">User that received the vlog like.</param>
        /// <param name="vlogLikeId">The vlog like id.</param>
        public virtual Task NotifyVlogLikedAsync(Guid receivingUserId, VlogLikeId vlogLikeId)
        {
            if (vlogLikeId is null)
            {
                throw new ArgumentNullException(nameof(vlogLikeId));
            }

            var notificationContext = _notificationFactory.BuildVlogGainedLike(receivingUserId, vlogLikeId.VlogId, vlogLikeId.UserId);

            _logger.LogTrace($"Notifying user {receivingUserId} that {vlogLikeId.VlogId} gained a like");

            _dispatchManager.Dispatch<NotifyBackgroundTask<DataVlogGainedLike>>(notificationContext);

            return Task.CompletedTask;
        }

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

        /// <summary>
        ///     Unregisters a user.
        /// </summary>
        /// <remarks>
        ///     For notifications, the userId and the notification
        ///     registration id are the same.
        /// </remarks>
        /// <param name="userId">The user to unregister.</param>
        public virtual async Task UnregisterAsync(Guid userId)
        {
            if (!await _notificationRegistrationRepository.ExistsAsync(userId))
            {
                // If we have no existing registrations we can simply return.
                // TODO Invalid op?
                return;
            }

            var registration = await _notificationRegistrationRepository.GetAsync(userId);

            await _notificationClient.UnregisterAsync(registration);

            await _notificationRegistrationRepository.DeleteAsync(userId);
        }
    }
}
