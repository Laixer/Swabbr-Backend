using Swabbr.Api.ViewModels.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Swabbr.Api.ViewModels.VlogLike
{
    /// <summary>
    ///     Output model for a vlog like summary.
    /// </summary>
    public sealed class VlogLikeSummaryOutputModel
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
        public IEnumerable<UserSimplifiedOutputModel> SimplifiedUsers { get; set; }
    }
}
