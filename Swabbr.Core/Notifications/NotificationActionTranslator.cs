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
            return action switch
            {
                NotificationAction.FollowedProfileLive => "followed_profile_live",
                NotificationAction.FollowedProfileVlogPosted => "followed_profile_vlog_posted",
                NotificationAction.InactiveUserMotivate => "inactive_user_motivate",
                NotificationAction.InactiveUnwatchedVlogs => "inactive_unwatched_vlogs",
                NotificationAction.InactiveVlogRecordRequest => "inactive_vlog_record_request",
                NotificationAction.VlogGainedLikes => "vlog_gained_likes",
                NotificationAction.VlogNewReaction => "vlog_new_reaction",
                NotificationAction.VlogRecordRequest => "vlog_record_request",
                _ => throw new InvalidOperationException(nameof(action)),
            };
        }

    }

}
