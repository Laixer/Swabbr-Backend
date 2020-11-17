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
        ///     Indicates that a followed profile is live.
        /// </summary>
        FollowedProfileLive,

        /// <summary>
        ///     Indicates a followed profile posted a new vlog.
        /// </summary>
        FollowedProfileVlogPosted,

        /// <summary>
        ///     Used to motivate the user to be more active.
        /// </summary>
        InactiveUserMotivate,

        /// <summary>
        ///     Indicates the user has unwatched new vlogs.
        /// </summary>
        InactiveUnwatchedVlogs,

        /// <summary>
        ///     Indicates the user should record more vlogs.
        /// </summary>
        InactiveVlogRecordRequest,

        /// <summary>
        ///     Indicates a user vlog gained a like.
        /// </summary>
        VlogGainedLikes,

        /// <summary>
        ///     Indicates a user vlog gained a reaction.
        /// </summary>
        VlogNewReaction,

        /// <summary>
        ///     Requests the user to start vlogging.
        /// </summary>
        VlogRecordRequest
    }
}
