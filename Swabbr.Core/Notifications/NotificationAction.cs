namespace Swabbr.Core.Notifications
{
    /// <summary>
    ///     Enum that represents all possible notification actions.
    /// </summary>
    /// <remarks>
    ///     This is mainly used to explicitly indicate the kind of 
    ///     notification to receiving parties.
    /// </remarks>
    public enum NotificationAction
    {
        /// <summary>
        ///     Indicates a followed profile posted a new vlog.
        /// </summary>
        FollowedProfileVlogPosted = 0,

        /// <summary>
        ///     Indicates a user vlog gained a like.
        /// </summary>
        VlogGainedLike = 1,

        /// <summary>
        ///     Indicates a user vlog gained a reaction.
        /// </summary>
        VlogNewReaction = 2,

        /// <summary>
        ///     Requests the user to start vlogging.
        /// </summary>
        VlogRecordRequest = 3
    }
}
