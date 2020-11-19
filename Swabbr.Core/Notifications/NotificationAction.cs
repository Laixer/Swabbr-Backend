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
        FollowedProfileLive = 0,

        /// <summary>
        ///     Indicates a followed profile posted a new vlog.
        /// </summary>
        FollowedProfileVlogPosted = 1,

        /// <summary>
        ///     Used to motivate the user to be more active.
        /// </summary>
        InactiveUserMotivate = 2,

        /// <summary>
        ///     Indicates the user has unwatched new vlogs.
        /// </summary>
        InactiveUnwatchedVlogs = 3,

        /// <summary>
        ///     Indicates the user should record more vlogs.
        /// </summary>
        InactiveVlogRecordRequest = 4,

        /// <summary>
        ///     Indicates a user vlog gained a like.
        /// </summary>
        VlogGainedLikes = 5,

        /// <summary>
        ///     Indicates a user vlog gained a reaction.
        /// </summary>
        VlogNewReaction = 6,

        /// <summary>
        ///     Requests the user to start vlogging.
        /// </summary>
        VlogRecordRequest = 7
    }
}
