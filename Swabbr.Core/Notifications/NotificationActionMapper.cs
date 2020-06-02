using System;

namespace Swabbr.Core.Notifications
{

    /// <summary>
    /// Maps between <see cref="NotificationAction"/> and string values of our
    /// <see cref="NotificationActionConstants"/> class.
    /// </summary>
    public static class NotificationActionMapper
    {

        public static string Map(NotificationAction notificationAction)
        {
            return notificationAction switch
            {
                NotificationAction.FollowedProfileLive => NotificationActionConstants.FollowedProfileLive,
                NotificationAction.InactiveUserMotivate => NotificationActionConstants.InactiveUserMotivate,
                NotificationAction.InactiveUnwatchedVlogs => NotificationActionConstants.InactiveUnwatchedVlogs,
                NotificationAction.InactiveVlogRecordRequest => NotificationActionConstants.InactiveVlogRecordRequest,
                NotificationAction.VlogGainedLikes => NotificationActionConstants.VlogGainedLikes,
                NotificationAction.VlogNewReaction => NotificationActionConstants.VlogNewReaction,
                NotificationAction.VlogRecordRequest => NotificationActionConstants.VlogRecordRequest,
                _ => throw new InvalidOperationException(nameof(notificationAction)),
            };
        }

    }

}
