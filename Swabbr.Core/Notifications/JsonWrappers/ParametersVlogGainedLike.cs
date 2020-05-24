using System;

namespace Swabbr.Core.Notifications.JsonWrappers
{

    /// <summary>
    /// Json wrapper for a <see cref="Notifications.NotificationAction.VlogGainedLikes"/> push.
    /// </summary>
    public sealed class ParametersVlogGainedLike : ParametersJsonBase
    {

        public Guid VlogId { get; set; }

        public Guid UserThatLikedId { get; set; }

    }

}
