using Microsoft.Extensions.Logging;
using Swabbr.Core.Interfaces.Clients;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Notifications.Data;
using Swabbr.Core.Types;
using System;
using System.Collections.Generic;

namespace Swabbr.Infrastructure.Notifications.BackgroundTasks
{
    /// <summary>
    ///     Background task notifying all followers of a user
    ///     when the user posts a new vlog.
    /// </summary>
    public class NotifyFollowersVlogPostedBackgroundTask : NotifyMultipleBackgroundTask<DataFollowedProfileVlogPosted>
    {
        private readonly IUserRepository _userRepository;

        /// <summary>
        ///     Create new instance.
        /// </summary>
        public NotifyFollowersVlogPostedBackgroundTask(INotificationClient notificationClient, 
            INotificationRegistrationRepository notificationRegistrationRepository,
            ILogger<NotifyBackgroundTask<DataFollowedProfileVlogPosted>> logger, 
            IUserRepository userRepository) 
            : base(notificationClient, notificationRegistrationRepository, logger, userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
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
