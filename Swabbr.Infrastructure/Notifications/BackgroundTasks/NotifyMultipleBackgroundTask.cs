using Microsoft.Extensions.Logging;
using Swabbr.Core.BackgroundWork;
using Swabbr.Core.Interfaces.Clients;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Notifications;
using Swabbr.Core.Notifications.Data;
using Swabbr.Core.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swabbr.Infrastructure.Notifications.BackgroundTasks
{
    /// <summary>
    ///     Background task notifying a collection of users.
    /// </summary>
    /// <remarks>
    ///     Implement this and specify <see cref="GetUsersAsync(TData)"/>
    ///     to specify which users should be notified. This enables us
    ///     to use the benefits of <see cref="IAsyncEnumerable{T}"/>.
    /// </remarks>
    /// <typeparam name="TData">Notification data type.</typeparam>
    public abstract class NotifyMultipleBackgroundTask<TData> : NotifyBackgroundTask<TData>
        where TData : NotificationData
    {
        // TODO Maybe protected INotificationClient (without readonly) in base?
        private readonly INotificationClient _notificationClient;
        private readonly ILogger<NotifyBackgroundTask<TData>> _logger;

        /// <summary>
        ///     Create new instance.
        /// </summary>
        public NotifyMultipleBackgroundTask(INotificationClient notificationClient, 
            INotificationRegistrationRepository notificationRegistrationRepository,
            ILogger<NotifyBackgroundTask<TData>> logger, 
            IUserRepository userRepository) 
            : base(notificationClient, notificationRegistrationRepository, logger, userRepository)
        {
            _notificationClient = notificationClient ?? throw new ArgumentNullException(nameof(notificationClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        ///     Checks if we can handle some object.
        /// </summary>
        /// <param name="value">The object to check.</param>
        public override bool CanHandle(object value)
            => value is NotificationContext c
                && !c.HasUser
                && c.Notification is not null
                && c.Notification.Data is TData;

        /// <summary>
        ///     Notify all users specified in <see cref="GetUsersAsync"/>.
        /// </summary>
        /// <param name="context">The background task context.</param>
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

            var notification = (SwabbrNotification)context.Value;
            var data = (TData)notification.Data;

            await foreach (var pushDetails in GetUsersAsync(data))
            {
                await _notificationClient.SendNotificationAsync(pushDetails, notification);
                _logger.LogTrace($"Sent notification to user {pushDetails.UserId}");
            }
        }

        /// <summary>
        ///     Gets all users which should be notified.
        /// </summary>
        /// <remarks>
        ///     How to get the users which have to be notified 
        ///     should always be extractable from the data object.
        /// </remarks>
        /// <param name="data">Notification data object.</param>
        /// <returns>Users to notify.</returns>
        protected abstract IAsyncEnumerable<UserPushNotificationDetails> GetUsersAsync(TData data);
    }
}
