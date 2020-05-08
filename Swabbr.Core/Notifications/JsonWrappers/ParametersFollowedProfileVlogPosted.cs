using System;

namespace Swabbr.Core.Notifications.JsonWrappers
{

    /// <summary>
    /// JSON wrapper for notifying about a posted <see cref="Core.Entities.Vlog"/>.
    /// </summary>
    public sealed class ParametersFollowedProfileVlogPosted : ParametersJsonBase
    {

        public Guid VlogId { get; set; }

        public Guid VlogOwnerUserId { get; set; }

    }

}
