using Swabbr.Core.Entities;
using Swabbr.Core.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.Repositories
{
    /// <summary>
    ///     Contract for a vlog like repository.
    /// </summary>
    public interface IVlogLikeRepository : IRepository<VlogLike, VlogLikeId>
    {
        /// <summary>
        ///     Gets all vlog likes for a vlog.
        /// </summary>
        /// <param name="vlogId">The vlog to get likes for.</param>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>Vlog likes for the vlog.</returns>
        IAsyncEnumerable<VlogLike> GetForVlogAsync(Guid vlogId, Navigation navigation);

        /// <summary>
        ///     Gets a vlog like summary for a vlog.
        /// </summary>
        /// <param name="vlogId">The vlog to summarize.</param>
        /// <returns>The vlog like summary.</returns>
        Task<VlogLikeSummary> GetSummaryForVlogAsync(Guid vlogId);
    }
}
