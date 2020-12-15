using Microsoft.Extensions.Logging;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Notifications.Data;
using Swabbr.Core.Types;
using System.Collections.Generic;

namespace Swabbr.Infrastructure.Notifications.BackgroundTasks
{
    // TODO Use NotificationContext.NotifiedUserId (rename to UserId) for this?
    /// <summary>
    ///     Background task notifying.
    /// </summary>
    public class NotifyFollowersVlogPostedBackgroundTask : NotifyMultipleBackgroundTask<DataFollowedProfileVlogPosted>
    {
        public NotifyFollowersVlogPostedBackgroundTask(NotificationClient notificationClient, 
            INotificationRegistrationRepository notificationRegistrationRepository,
            ILogger<NotifyBackgroundTask<DataFollowedProfileVlogPosted>> logger, 
            IUserRepository userRepository) 
            : base(notificationClient, notificationRegistrationRepository, logger, userRepository)
        {
        }

        protected override IAsyncEnumerable<UserPushNotificationDetails> GetUsersAsync(DataFollowedProfileVlogPosted data)
            => _userRepository.GetFollowersPushDetailsAsync(data.VlogOwnerUserId, Navigation.All);
    }
}
