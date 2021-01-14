using Swabbr.Core.Entities;
using System;
using System.Collections.Generic;

namespace Swabbr.Core.Types
{
    /// <summary>
    ///     Summary wrapper for the likes for a vlog.
    /// </summary>
    public sealed class VlogLikeSummary
    {
        /// <summary>
        ///     The vlog id to which this wrapper belongs.
        /// </summary>
        public Guid VlogId { get; set; }

        /// <summary>
        ///     The total amount of likes for this vlog.
        /// </summary>
        public uint TotalLikes { get; set; }

        /// <summary>
        ///     The simplified users that liked this vlog.
        /// </summary>
        /// <remarks>
        ///     This does not need to contain all the users.
        /// </remarks>
        public IEnumerable<User> Users { get; set; }
    }
}
