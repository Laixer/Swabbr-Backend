using System;

namespace Swabbr.Core.Notifications
{
    // TODO Should we use this?
    /// <summary>
    ///     Maps from <see cref="NotificationAction"/> to the 
    ///     corresponding string value.
    /// </summary>
    public static class NotificationActionTranslator
    {
        /// <summary>
        ///     Translates a notification action to the 
        ///     corresponding string value.
        /// </summary>
        /// <param name="action">The notification action.</param>
        /// <returns>Translated string value.</returns>
        public static string Translate(NotificationAction action) => action switch
        {
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
