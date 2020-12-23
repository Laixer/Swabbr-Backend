using System;

namespace Swabbr.Core.Context
{
    /// <summary>
    ///     Context for posting a vlog using 
    ///     a background task.
    /// </summary>
    public class PostVlogContext
    {
        /// <summary>
        ///     The user that posts the vlog.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        ///     The suggestive id of the vlog.
        /// </summary>
        public Guid VlogId { get; set; }

        /// <summary>
        ///     Indicates whether the vlog is public or private.
        /// </summary>
        public bool IsPrivate { get; set; }
    }
}
