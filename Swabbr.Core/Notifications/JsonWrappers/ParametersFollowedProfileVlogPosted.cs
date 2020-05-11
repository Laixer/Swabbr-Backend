using System;

namespace Swabbr.Core.Notifications.JsonWrappers
{

    /// <summary>
    /// JSON wrapper for notifying about a posted <see cref="Entities.Vlog"/>.
    /// </summary>
    public sealed class ParametersFollowedProfileVlogPosted : ParametersJsonBase
    {

        /// <summary>
        /// Internal <see cref="Entities.Vlog"/> id.
        /// </summary>
        public Guid VlogId { get; set; }

        /// <summary>
        /// Internal <see cref="Entities.SwabbrUser"/> id of the person that owns
        /// the vlog.
        /// </summary>
        public Guid VlogOwnerUserId { get; set; }

    }

}
