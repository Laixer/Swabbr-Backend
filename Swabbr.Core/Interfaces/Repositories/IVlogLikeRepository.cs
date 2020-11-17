using Swabbr.Core.Entities;
using Swabbr.Core.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.Repositories
{
    /// <summary>
    ///     Contract for a <see cref="VlogLike"/> repository.
    /// </summary>
    public interface IVlogLikeRepository : IRepository<VlogLike, VlogLikeId>, ICudFunctionality<VlogLike, VlogLikeId>
    {
        Task<bool> ExistsAsync(VlogLikeId vlogLikeId);

        Task<IEnumerable<VlogLike>> GetAllForVlogAsync(Guid vlogId);

        /// <summary>
        ///     Gets a <see cref="VlogLikeSummary"/> for a <see cref="Vlog"/>.
        /// </summary>
        /// <remarks>
        ///     The <see cref="VlogLikeSummary.Users"/> field does not
        ///     need to contain all users that liked the <see cref="Vlog"/>.
        /// </remarks>
        /// <param name="vlogId">Internal <see cref="Vlog"/> id</param>
        /// <returns><see cref="VlogLikeSummary"/></returns>
        Task<VlogLikeSummary> GetVlogLikeSummaryForVlogAsync(Guid vlogId);

        Task<int> GetCountForVlogAsync(Guid vlogId);
    }
}