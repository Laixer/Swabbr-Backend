using System;

namespace Swabbr.Core.Types
{
    /// <summary>
    ///     Primary key object containing both a
    ///     user id and a vlog id.
    /// </summary>
    public sealed class VlogLikeId
    {
        /// <summary>
        ///     The internal vlog id.
        /// </summary>
        public Guid VlogId { get; set; }

        /// <summary>
        ///     The internal user id.
        /// </summary>
        public Guid UserId { get; set; }
    }
}
