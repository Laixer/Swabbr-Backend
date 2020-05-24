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
            switch (notificationAction)
            {
                case NotificationAction.FollowedProfileLive:
                    return NotificationActionConstants.FollowedProfileLive;
                case NotificationAction.InactiveUserMotivate:
                    return NotificationActionConstants.InactiveUserMotivate;
                case NotificationAction.InactiveUnwatchedVlogs:
                    return NotificationActionConstants.InactiveUnwatchedVlogs;
                case NotificationAction.InactiveVlogRecordRequest:
                    return NotificationActionConstants.InactiveVlogRecordRequest;
                case NotificationAction.VlogGainedLikes:
                    return NotificationActionConstants.VlogGainedLikes;
                case NotificationAction.VlogNewReaction:
                    return NotificationActionConstants.VlogNewReaction;
                case NotificationAction.VlogRecordRequest:
                    return NotificationActionConstants.VlogRecordRequest;
            }

            throw new InvalidOperationException(nameof(notificationAction));
        }

    }

}
