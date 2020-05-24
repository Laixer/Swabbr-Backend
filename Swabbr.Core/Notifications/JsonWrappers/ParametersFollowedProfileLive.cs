using System;

namespace Swabbr.Core.Notifications.JsonWrappers
{

    /// <summary>
    /// JSON wrapper for a notification of a currently live profile.
    /// </summary>
    public sealed class ParametersFollowedProfileLive : ParametersJsonBase
    {

        /// <summary>
        /// Internal <see cref="Entities.SwabbrUser"/> id of the person that is live.
        /// </summary>
        public Guid LiveUserId { get; set; }

        /// <summary>
        /// Internal <see cref="Entities.Livestream"/> id.
        /// </summary>
        public Guid LiveLivestreamId { get; set; }

        /// <summary>
        /// Internal <see cref="Entities.Vlog"/> id.
        /// </summary>
        public Guid LiveVlogId { get; set; }

    }

}
