using Microsoft.Extensions.Logging;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Notifications.Data;
using Swabbr.Core.Types;
using System.Collections.Generic;

namespace Swabbr.Infrastructure.Notifications.BackgroundTasks
{
    // TODO Use NotificationContext.NotifiedUserId (rename to UserId) for this?
    /// <summary>
    ///     Background task notifying all followers of a user
    ///     when the user posts a new vlog.
    /// </summary>
    public class NotifyFollowersVlogPostedBackgroundTask : NotifyMultipleBackgroundTask<DataFollowedProfileVlogPosted>
    {
        /// <summary>
        ///     Create new instance.
        /// </summary>
        public NotifyFollowersVlogPostedBackgroundTask(NotificationClient notificationClient, 
            INotificationRegistrationRepository notificationRegistrationRepository,
            ILogger<NotifyBackgroundTask<DataFollowedProfileVlogPosted>> logger, 
            IUserRepository userRepository) 
            : base(notificationClient, notificationRegistrationRepository, logger, userRepository)
        {
        }

        /// <summary>
        ///     Gets all following users.
        /// </summary>
        /// <param name="data">Notification data.</param>
        /// <returns>Followers push details.</returns>
        protected override IAsyncEnumerable<UserPushNotificationDetails> GetUsersAsync(DataFollowedProfileVlogPosted data)
            => _userRepository.GetFollowersPushDetailsAsync(data.VlogOwnerUserId, Navigation.All);
    }
}
