using System;

namespace Swabbr.Core.Notifications.JsonWrappers
{
    /// <summary>
    ///     JSON wrapper for a notification indicating a vlog
    ///     gained a like..
    /// </summary>
    public sealed class ParametersVlogGainedLike : ParametersJsonBase
    {
        /// <summary>
        ///     The vlog that gained the like.
        /// </summary>
        public Guid VlogId { get; set; }

        /// <summary>
        ///     The user that liked the vlog.
        /// </summary>
        public Guid UserThatLikedId { get; set; }
    }
}
