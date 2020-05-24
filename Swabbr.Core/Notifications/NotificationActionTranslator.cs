using System;

namespace Swabbr.Core.Notifications
{

    /// <summary>
    /// Maps from <see cref="NotificationAction"/> to the corresponding string value.
    /// </summary>
    public static class NotificationActionTranslator
    {

        public static string Translate(NotificationAction action)
        {
            switch (action)
            {
                case NotificationAction.FollowedProfileLive:
                    return "followed_profile_live";
                case NotificationAction.FollowedProfileVlogPosted:
                    return "followed_profile_vlog_posted";
                case NotificationAction.InactiveUserMotivate:
                    return "inactive_user_motivate";
                case NotificationAction.InactiveUnwatchedVlogs:
                    return "inactive_unwatched_vlogs";
                case NotificationAction.InactiveVlogRecordRequest:
                    return "inactive_vlog_record_request";
                case NotificationAction.VlogGainedLikes:
                    return "vlog_gained_likes";
                case NotificationAction.VlogNewReaction:
                    return "vlog_new_reaction";
                case NotificationAction.VlogRecordRequest:
                    return "vlog_record_request";
            }

            throw new InvalidOperationException(nameof(action));
        }

    }

}
