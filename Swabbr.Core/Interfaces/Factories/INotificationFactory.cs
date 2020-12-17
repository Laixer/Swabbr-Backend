using Swabbr.Core.Notifications;
using System;

namespace Swabbr.Core.Interfaces.Factories
{
    /// <summary>
    ///     Contract for creating notifications in a 
    ///     <see cref="NotificationContext"/>.
    /// </summary>
    public interface INotificationFactory
    {
        /// <summary>
        ///     Build a notification for indicating that a followed
        ///     profile has posted a vlog.
        /// </summary>
        /// <param name="vlogId">The posted vlog.</param>
        /// <param name="vlogOwnerUserId">The vlog owner.</param>
        /// <returns>Notification object.</returns>
        public NotificationContext BuildFollowedProfileVlogPosted(Guid vlogId, Guid vlogOwnerUserId);

        /// <summary>
        ///     Build a notification for indicating that a user
        ///     should start recording a vlog.
        /// </summary>
        /// <param name="notifiedUserId">The notified user.</param>
        /// <param name="vlogId">The vlog id.</param>
        /// <param name="requestMoment">The moment of request.</param>
        /// <param name="requestTimeout">The timeout of the request.</param>
        /// <returns>Notification object.</returns>
        public NotificationContext BuildRecordVlog(Guid notifiedUserId, Guid vlogId, DateTimeOffset requestMoment, TimeSpan requestTimeout);

        /// <summary>
        ///     Build a notification for indicating that a vlog
        ///     gained a like by some user.
        /// </summary>
        /// <param name="notifiedUserId">Notified user.</param>
        /// <param name="vlogId">The vlog that was liked.</param>
        /// <param name="userThatLikedId">The user that liked.</param>
        /// <returns>Notification object.</returns>
        public NotificationContext BuildVlogGainedLike(Guid notifiedUserId, Guid vlogId, Guid userThatLikedId);

        /// <summary>
        ///     Build a notification for indicating that a user
        ///     posted a reaction to a given vlog.
        /// </summary>
        /// <param name="notifiedUserId">Notified user.</param>
        /// <param name="vlogId">The vlog to which was reacted.</param>
        /// <param name="reactionId">The reaction id.</param>
        /// <returns>Notification object.</returns>
        public NotificationContext BuildVlogNewReaction(Guid notifiedUserId, Guid vlogId, Guid reactionId);
    }
}
