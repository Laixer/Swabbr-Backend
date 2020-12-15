using Microsoft.Extensions.Logging;
using Swabbr.Core.BackgroundWork;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Notifications;
using Swabbr.Core.Notifications.Data;
using System;
using System.Threading.Tasks;

namespace Swabbr.Infrastructure.Notifications.BackgroundTasks
{
    // TODO Notification generics with TData is not that elegant
    /// <summary>
    ///     Background task notifying a single user.
    /// </summary>
    /// <typeparam name="TData">The notification data type.</typeparam>
    public class NotifyBackgroundTask<TData> : BackgroundTask
        where TData : NotificationData
    {
        protected readonly NotificationClient _notificationClient;
        protected readonly INotificationRegistrationRepository _notificationRegistrationRepository;
        protected readonly IUserRepository _userRepository; // TODO Two repos for sort of the same thing --> seems incorrect.
        protected readonly ILogger<NotifyBackgroundTask<TData>> _logger;

        /// <summary>
        ///     Create new instance.
        /// </summary>
        public NotifyBackgroundTask(NotificationClient notificationClient,
            INotificationRegistrationRepository notificationRegistrationRepository,
            ILogger<NotifyBackgroundTask<TData>> logger, 
            IUserRepository userRepository)
        {
            _notificationClient = notificationClient ?? throw new ArgumentNullException(nameof(notificationClient));
            _notificationRegistrationRepository = notificationRegistrationRepository ?? throw new ArgumentNullException(nameof(notificationRegistrationRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        /// <summary>
        ///     Checks if we can handle the context value object.
        /// </summary>
        /// <param name="value">The object to check.</param>
        public override bool CanHandle(object value)
            => value is NotificationContext c 
                && c.HasUser 
                && c.Notification is not null 
                && c.Notification.Data is TData;

        /// <summary>
        ///     Notify a single user.
        /// </summary>
        /// <param name="context">The background context.</param>
        public override async Task ExecuteAsync(BackgroundTaskContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            if (!CanHandle(context.Value))
            {
                throw new InvalidOperationException(nameof(context));
            }

            var notificationContext = (NotificationContext)context.Value;

            // Check if we can reach the user that should be notified.
            if (!await _notificationRegistrationRepository.ExistsAsync(notificationContext.NotifiedUserId))
            {
                _logger.LogTrace($"Couldn't get notification registration for user {notificationContext.NotifiedUserId}");
                return;
            }

            var pushDetails = await _userRepository.GetPushDetailsAsync(notificationContext.NotifiedUserId);
            await _notificationClient.SendNotificationAsync(pushDetails.UserId, pushDetails.PushNotificationPlatform, notificationContext.Notification);

            _logger.LogTrace($"Sent notification to user {notificationContext.NotifiedUserId}");
        }
    }
}
