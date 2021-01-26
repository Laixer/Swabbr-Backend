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
            NotificationAction.FollowedProfileVlogPosted => "FOLLOWED_PROFILE_VLOG_POSTED",
            NotificationAction.VlogGainedLike => "VLOG_GAINED_LIKE",
            NotificationAction.VlogNewReaction => "VLOG_NEW_REACTION",
            NotificationAction.VlogRecordRequest => "VLOG_RECORD_REQUEST",
            _ => throw new InvalidOperationException(nameof(action)),
        };
    }
}
