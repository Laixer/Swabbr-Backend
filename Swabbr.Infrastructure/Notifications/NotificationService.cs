using Swabbr.Core.BackgroundWork;
using Swabbr.Core.Entities;
using Swabbr.Core.Interfaces.Clients;
using Swabbr.Core.Interfaces.Factories;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Interfaces.Services;
using Swabbr.Core.Notifications.Data;
using Swabbr.Core.Types;
using Swabbr.Infrastructure.Notifications.BackgroundTasks;
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
        private readonly INotificationFactory _notificationFactory;

        /// <summary>
        ///     Create new instance.
        /// </summary>
        public NotificationService(INotificationClient notificationClient,
            INotificationRegistrationRepository notificationRegistrationRepository,
            DispatchManager dispatchManager,
            INotificationFactory notificationFactory)
        {
            _notificationClient = notificationClient ?? throw new ArgumentNullException(nameof(notificationClient));
            _notificationRegistrationRepository = notificationRegistrationRepository ?? throw new ArgumentNullException(nameof(notificationRegistrationRepository));
            _dispatchManager = dispatchManager ?? throw new ArgumentNullException(nameof(dispatchManager));
            _notificationFactory = notificationFactory ?? throw new ArgumentNullException(nameof(notificationFactory));
        }

        /// <summary>
        ///     Checks if the notification service is online.
        /// </summary>
        public virtual Task<bool> IsServiceOnlineAsync()
            => _notificationClient.IsServiceAvailableAsync();

        /// <summary>
        ///     Notify all followers of a user that a new vlog was posted.
        /// </summary>
        /// <remarks>
        ///     This is dispatched to the <see cref="DispatchManager"/>.
        /// </remarks>
        /// <param name="userId">User that posted a vlog.</param>
        /// <param name="vlogId">The posted vlog id.</param>
        public virtual Task NotifyFollowersVlogPostedAsync(Guid userId, Guid vlogId)
        {
            var notification = NotificationFactory.BuildFollowedProfileVlogPosted(userId, vlogId);

            _dispatchManager.Dispatch<NotifyFollowersVlogPostedBackgroundTask>(notification);

            return Task.CompletedTask;
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
            var notification = NotificationFactory.BuildRecordVlog(userId, vlogId, DateTimeOffset.Now, requestTimeout);

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
            var notificationContext = NotificationFactory.BuildVlogNewReaction(receivingUserId, vlogId, reactionId);

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

            var notificationContext = NotificationFactory.BuildVlogGainedLike(receivingUserId, vlogLikeId.VlogId, vlogLikeId.UserId);

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

        // FUTURE: Implement.
        public virtual Task UnregisterAsync(Guid userId)
            => throw new NotImplementedException();
    }
}
