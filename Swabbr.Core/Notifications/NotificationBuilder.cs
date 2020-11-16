using Swabbr.Core.Extensions;
using Swabbr.Core.Interfaces.Notifications;
using Swabbr.Core.Notifications.JsonWrappers;
using Swabbr.Core.Utility;
using System;

namespace Swabbr.Core.Notifications
{

    /// <summary>
    /// Builds <see cref="SwabbrNotification"/>s for us.
    /// TODO Titles.
    /// </summary>
    public sealed class NotificationBuilder : INotificationBuilder
    {

        public SwabbrNotification BuildFollowedProfileLive(Guid liveUserId, Guid livestreamId, Guid liveVlogId)
        {
            liveUserId.ThrowIfNullOrEmpty();
            livestreamId.ThrowIfNullOrEmpty();
            liveVlogId.ThrowIfNullOrEmpty();

            return new SwabbrNotification(NotificationAction.FollowedProfileLive, new ParametersFollowedProfileLive
            {
                LiveLivestreamId = livestreamId,
                LiveUserId = liveUserId,
                LiveVlogId = liveVlogId
            }, title: NotificationTextData.DefaultTitle, message: NotificationTextData.DefaultTitle);
        }

        public SwabbrNotification BuildFollowedProfileVlogPosted(Guid vlogId, Guid vlogOwnerUserId)
        {
            vlogId.ThrowIfNullOrEmpty();
            vlogOwnerUserId.ThrowIfNullOrEmpty();

            return new SwabbrNotification(NotificationAction.FollowedProfileVlogPosted, new ParametersFollowedProfileVlogPosted
            {
                VlogId = vlogId,
                VlogOwnerUserId = vlogOwnerUserId
            }, title: NotificationTextData.DefaultTitle, message: NotificationTextData.DefaultTitle);
        }

        public SwabbrNotification BuildRecordVlog(Guid livestreamId, Guid vlogId, DateTimeOffset requestMoment, TimeSpan requestTimeout)
        {
            livestreamId.ThrowIfNullOrEmpty();
            vlogId.ThrowIfNullOrEmpty();
            requestMoment.ThrowIfNullOrEmpty();
            // TODO Timespan nullcheck

            return new SwabbrNotification(NotificationAction.VlogRecordRequest, new ParametersRecordVlog
            {
                LivestreamId = livestreamId,
                RequestMoment = requestMoment,
                RequestTimeout = requestTimeout,
                VlogId = vlogId
            }, title: NotificationTextData.DefaultTitle, message: NotificationTextData.DefaultTitle);
        }

        public SwabbrNotification BuildVlogGainedLike(Guid vlogId, Guid userThatLikedId)
        {
            vlogId.ThrowIfNullOrEmpty();
            userThatLikedId.ThrowIfNullOrEmpty();

            return new SwabbrNotification(NotificationAction.VlogGainedLikes, new ParametersVlogGainedLike
            {
                UserThatLikedId = userThatLikedId,
                VlogId = vlogId
            }, title: NotificationTextData.DefaultTitle, message: NotificationTextData.DefaultTitle);
        }

        public SwabbrNotification BuildVlogNewReaction(Guid vlogId, Guid reactionId)
        {
            vlogId.ThrowIfNullOrEmpty();
            reactionId.ThrowIfNullOrEmpty();

            return new SwabbrNotification(NotificationAction.VlogNewReaction, new ParametersVlogNewReaction
            {
                ReactionId = reactionId,
                VlogId = vlogId
            }, title: NotificationTextData.DefaultTitle, message: NotificationTextData.DefaultTitle);
        }

    }

}
