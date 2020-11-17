using Swabbr.Core.Entities;
using Swabbr.Core.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.Services
{

    /// <summary>
    /// Service for processing <see cref="Vlog"/> and <see cref="VlogLike"/> 
    /// related requests.
    /// </summary>
    public interface IVlogService
    {

        Task AddView(Guid vlogId);

        Task<Vlog> GetAsync(Guid vlogId);

        Task<bool> ExistsAsync(Guid vlogId);

        /// <summary>
        ///     Gets all the <see cref="VlogLike"/>s for a <see cref="Vlog"/>.
        /// </summary>
        /// <remarks>
        ///     This does not scale. If that is required, use an implementation
        ///     of <see cref="GetVlogLikeSummaryForVlogAsync(Guid)"/> which does
        ///     not return all <see cref="VlogLike"/> but only a subset.
        /// </remarks>
        /// <param name="vlogId">Internal <see cref="Vlog"/> id</param>
        /// <returns><see cref="VlogLike"/> collection</returns>
        Task<IEnumerable<VlogLike>> GetAllVlogLikesForVlogAsync(Guid vlogId);

        /// <summary>
        ///     Gets a <see cref="VlogLikeSummary"/> for a given vlog.
        /// </summary>
        /// <remarks>
        ///     The <see cref="VlogLikeSummary.Users"/> does not need
        ///     to contain all the <see cref="SwabbrUser"/> entries.
        /// </remarks>
        /// <param name="vlogId">Internal <see cref="Vlog"/> id</param>
        /// <returns><see cref="VlogLikeSummary"/></returns>
        Task<VlogLikeSummary> GetVlogLikeSummaryForVlogAsync(Guid vlogId);

        Task<Vlog> GetVlogFromLivestreamAsync(Guid livestreamId);

        Task<Vlog> UpdateAsync(Guid vlogId, Guid userId, bool isPrivate);

        Task<IEnumerable<Vlog>> GetVlogsFromUserAsync(Guid userId);

        Task DeleteAsync(Guid vlogId, Guid userId);

        Task LikeAsync(Guid vlogId, Guid userId);

        Task UnlikeAsync(Guid vlogId, Guid userId);

        Task<IEnumerable<Vlog>> GetRecommendedForUserAsync(Guid userId, uint maxCount);

    }

}
