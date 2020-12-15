using System;

namespace Swabbr.Core.Notifications.Data
{
    /// <summary>
    ///     JSON Wrapper for a notification indicating that a vlog
    ///     received a new reaction.
    /// </summary>
    public sealed class DataVlogNewReaction : NotificationData
    {
        /// <summary>
        ///     The vlog that received the reaction.
        /// </summary>
        public Guid VlogId { get; set; }

        /// <summary>
        ///     The reaction that was posted.
        /// </summary>
        public Guid ReactionId { get; set; }
    }
}
