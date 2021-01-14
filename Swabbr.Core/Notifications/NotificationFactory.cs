using Swabbr.Core.Interfaces.Factories;
using Swabbr.Core.Notifications.Data;
using System;

namespace Swabbr.Core.Notifications
{
    /// <summary>
    ///     Builds <see cref="SwabbrNotification"/>s for us
    ///     wrapped in a <see cref="NotificationContext"/>.
    /// </summary>
    public class NotificationFactory : INotificationFactory
    {
        internal static string DefaultTitle => "Default notification title";
        internal static string DefaultMessage => "Default notification message";

        /// <summary>
        ///     Build a notification for indicating that a followed
        ///     profile has posted a vlog.
        /// </summary>
        /// <param name="vlogId">The posted vlog.</param>
        /// <param name="vlogOwnerUserId">The vlog owner.</param>
        /// <returns>Notification object.</returns>
        public virtual NotificationContext BuildFollowedProfileVlogPosted(Guid vlogId, Guid vlogOwnerUserId)
        {
            var context = DefaultContext();

            context.Notification = new SwabbrNotification(
                notificationAction: NotificationAction.FollowedProfileVlogPosted,
                data: new DataFollowedProfileVlogPosted
                {
                    VlogId = vlogId,
                    VlogOwnerUserId = vlogOwnerUserId,
                }, 
                title: DefaultTitle, 
                message: DefaultMessage);

            return context;
        }

        /// <summary>
        ///     Build a notification for indicating that a user
        ///     should start recording a vlog.
        /// </summary>
        /// <param name="notifiedUserId">The notified user.</param>
        /// <param name="vlogId">The vlog id.</param>
        /// <param name="requestMoment">The moment of request.</param>
        /// <param name="requestTimeout">The timeout of the request.</param>
        /// <returns>Notification object.</returns>
        public virtual NotificationContext BuildVlogRecordRequest(Guid notifiedUserId, Guid vlogId, DateTimeOffset requestMoment, TimeSpan requestTimeout)
        {
            var context = DefaultContext(notifiedUserId);

            context.Notification = new SwabbrNotification(
                notificationAction: NotificationAction.VlogRecordRequest,
                data: new DataVlogRecordRequest
                {
                    RequestMoment = requestMoment,
                    RequestTimeout = requestTimeout,
                    VlogId = vlogId,
                }, 
                title: DefaultTitle, 
                message: DefaultMessage);

            return context;
        }

        /// <summary>
        ///     Build a notification for indicating that a vlog
        ///     gained a like by some user.
        /// </summary>
        /// <param name="notifiedUserId">Notified user.</param>
        /// <param name="vlogId">The vlog that was liked.</param>
        /// <param name="userThatLikedId">The user that liked.</param>
        /// <returns>Notification object.</returns>
        public virtual NotificationContext BuildVlogGainedLike(Guid notifiedUserId, Guid vlogId, Guid userThatLikedId)
        {
            var context = DefaultContext(notifiedUserId);

            context.Notification = new SwabbrNotification(
                notificationAction: NotificationAction.VlogGainedLikes,
                data: new DataVlogGainedLike
                {
                    UserThatLikedId = userThatLikedId,
                    VlogId = vlogId,
                }, 
                title: DefaultTitle, 
                message: DefaultMessage);

            return context;
        }

        /// <summary>
        ///     Build a notification for indicating that a user
        ///     posted a reaction to a given vlog.
        /// </summary>
        /// <param name="notifiedUserId">Notified user.</param>
        /// <param name="vlogId">The vlog to which was reacted.</param>
        /// <param name="reactionId">The reaction id.</param>
        /// <returns>Notification object.</returns>
        public virtual NotificationContext BuildVlogNewReaction(Guid notifiedUserId, Guid vlogId, Guid reactionId)
        {
            var context = DefaultContext(notifiedUserId);

            context.Notification = new SwabbrNotification(NotificationAction.VlogNewReaction, new DataVlogNewReaction
            {
                ReactionId = reactionId,
                VlogId = vlogId,
            }, title: DefaultTitle, message: DefaultMessage);

            return context;
        }

        /// <summary>
        ///     Creates a default context with a user assigned if specified.
        /// </summary>
        /// <remarks>
        ///     Leave the <paramref name="notifiedUserId"/> unspecified when
        ///     notifying multiple users.
        /// </remarks>
        /// <param name="notifiedUserId">Notified user if it's a single user.</param>
        /// <returns>Context object.</returns>
        protected virtual NotificationContext DefaultContext(Guid? notifiedUserId = null)
            => new NotificationContext 
            {
                DateSent = DateTimeOffset.Now,
                NotifiedUserId = notifiedUserId ?? Guid.Empty,
            };
    }
}
