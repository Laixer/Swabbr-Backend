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
                    return NotificationActionConstants.FOLLOWED_PROFILE_LIVE;
                case NotificationAction.InactiveUserMotivate:
                    return NotificationActionConstants.INACTIVE_USER_MOTIVATE;
                case NotificationAction.InactiveUnwatchedVlogs:
                    return NotificationActionConstants.INACTIVE_UNWATCHED_VLOGS;
                case NotificationAction.InactiveVlogRecordRequest:
                    return NotificationActionConstants.INACTIVE_VLOG_RECORD_REQUEST;
                case NotificationAction.VlogGainedLikes:
                    return NotificationActionConstants.VLOG_GAINED_LIKES;
                case NotificationAction.VlogNewReaction:
                    return NotificationActionConstants.VLOG_NEW_REACTION;
                case NotificationAction.VlogRecordRequest:
                    return NotificationActionConstants.VLOG_RECORD_REQUEST;
            }

            throw new InvalidOperationException(nameof(notificationAction));
        }

    }

}
