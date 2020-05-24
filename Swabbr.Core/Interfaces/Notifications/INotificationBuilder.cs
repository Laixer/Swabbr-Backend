using Swabbr.Core.Notifications;
using System;

namespace Swabbr.Core.Interfaces.Notifications
{

    /// <summary>
    /// Contract for creationg <see cref="SwabbrNotification"/> objects.
    /// </summary>
    public interface INotificationBuilder
    {

        SwabbrNotification BuildFollowedProfileLive(Guid liveUserId, Guid livestreamId, Guid liveVlogId);

        SwabbrNotification BuildFollowedProfileVlogPosted(Guid vlogId, Guid vlogOwnerUserId);

        SwabbrNotification BuildRecordVlog(Guid livestreamId, Guid vlogId, DateTimeOffset requestMoment, TimeSpan requestTimeout);

        SwabbrNotification BuildVlogGainedLike(Guid vlogId, Guid userThatLikedId);

        SwabbrNotification BuildVlogNewReaction(Guid vlogId, Guid reactionId);

    }

}
