using System;

namespace Swabbr.Core.Notifications.JsonWrappers
{
    /// <summary>
    ///     JSON wrapper for notifying about a posted vlog.
    /// </summary>
    public sealed class ParametersFollowedProfileVlogPosted : ParametersJsonBase
    {
        /// <summary>
        /// Internal vlog id.
        /// </summary>
        public Guid VlogId { get; set; }

        /// <summary>
        /// Internal user id of the person that owns the vlog.
        /// </summary>
        public Guid VlogOwnerUserId { get; set; }
    }
}
