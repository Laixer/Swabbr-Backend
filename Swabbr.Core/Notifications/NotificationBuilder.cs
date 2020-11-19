using Swabbr.Core.Notifications.JsonWrappers;
using System;

namespace Swabbr.Core.Notifications
{
    // FUTURE Titles.
    /// <summary>
    ///     Builds <see cref="SwabbrNotification"/>s for us.
    /// </summary>
    public static class NotificationBuilder
    {
        internal static string DefaultTitle => "Default notification title";
        internal static string DefaultMessage => "Default notification message";

        /// <summary>
        ///     Build a notification for indicating that a followed
        ///     profile is currently livestreaming.
        /// </summary>
        /// <param name="liveUserId">The user that is live.</param>
        /// <param name="livestreamId">The corresponding livestream.</param>
        /// <param name="liveVlogId">The corresponding vlog.</param>
        /// <returns>Notification object.</returns>
        public static SwabbrNotification BuildFollowedProfileLive(Guid liveUserId, Guid livestreamId, Guid liveVlogId)
            => new SwabbrNotification(NotificationAction.FollowedProfileLive, new ParametersFollowedProfileLive
            {
                LiveLivestreamId = livestreamId,
                LiveUserId = liveUserId,
                LiveVlogId = liveVlogId
            }, title: DefaultTitle, message: DefaultTitle);

        /// <summary>
        ///     Build a notification for indicating that a followed
        ///     profile has posted a vlog.
        /// </summary>
        /// <param name="vlogId">The posted vlog.</param>
        /// <param name="vlogOwnerUserId">The vlog owner.</param>
        /// <returns>Notification object.</returns>
        public static SwabbrNotification BuildFollowedProfileVlogPosted(Guid vlogId, Guid vlogOwnerUserId)
            => new SwabbrNotification(NotificationAction.FollowedProfileVlogPosted, new ParametersFollowedProfileVlogPosted
            {
                VlogId = vlogId,
                VlogOwnerUserId = vlogOwnerUserId
            }, title: DefaultTitle, message: DefaultTitle);

        /// <summary>
        ///     Build a notification for indicating that a user
        ///     should start recording a vlog.
        /// </summary>
        /// <param name="livestreamId">The livestream id.</param>
        /// <param name="vlogId">The vlog id.</param>
        /// <param name="requestMoment">The moment of request.</param>
        /// <param name="requestTimeout">The timeout of the request.</param>
        /// <returns>Notification object.</returns>
        public static SwabbrNotification BuildRecordVlog(Guid livestreamId, Guid vlogId, DateTimeOffset requestMoment, TimeSpan requestTimeout)
            => new SwabbrNotification(NotificationAction.VlogRecordRequest, new ParametersRecordVlog
            {
                LivestreamId = livestreamId,
                RequestMoment = requestMoment,
                RequestTimeout = requestTimeout,
                VlogId = vlogId
            }, title: DefaultTitle, message: DefaultTitle);

        /// <summary>
        ///     Build a notification for indicating that a vlog
        ///     gained a like by some user.
        /// </summary>
        /// <param name="vlogId">The vlog that was liked.</param>
        /// <param name="userThatLikedId">The user that liked.</param>
        /// <returns>Notification object.</returns>
        public static SwabbrNotification BuildVlogGainedLike(Guid vlogId, Guid userThatLikedId)
            => new SwabbrNotification(NotificationAction.VlogGainedLikes, new ParametersVlogGainedLike
            {
                UserThatLikedId = userThatLikedId,
                VlogId = vlogId
            }, title: DefaultTitle, message: DefaultTitle);

        /// <summary>
        ///     Build a notification for indicating that a user
        ///     posted a reaction to a given vlog.
        /// </summary>
        /// <param name="vlogId">The vlog to which was reacted.</param>
        /// <param name="reactionId">The reaction id.</param>
        /// <returns>Notification object.</returns>
        public static SwabbrNotification BuildVlogNewReaction(Guid vlogId, Guid reactionId)
            => new SwabbrNotification(NotificationAction.VlogNewReaction, new ParametersVlogNewReaction
            {
                ReactionId = reactionId,
                VlogId = vlogId
            }, title: DefaultTitle, message: DefaultTitle);
    }
}
