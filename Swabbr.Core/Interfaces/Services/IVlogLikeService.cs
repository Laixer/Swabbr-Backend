using Swabbr.Core.Entities;
using Swabbr.Core.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.Services
{
    /// <summary>
    ///     Service for processing vlog like operations.
    /// </summary>
    /// <remarks>
    ///     The executing user id is never passed. Whenever 
    ///     possible, this id is extracted from the context.
    /// </remarks>
    public interface IVlogLikeService
    {
        /// <summary>
        ///     Checks if a vlog like exists in our data store.
        /// </summary>
        /// <param name="vlogLikeId">The vlog like id to check.</param>
        Task<bool> ExistsAsync(VlogLikeId vlogLikeId);

        /// <summary>
        ///     Gets a vlog like from our data store.
        /// </summary>
        /// <param name="vlogLikeId">The vlog like id.</param>
        /// <returns>The vlog like.</returns>
        Task<VlogLike> GetAsync(VlogLikeId vlogLikeId);

        /// <summary>
        ///     Gets the likes for a vlog.
        /// </summary>
        /// <remarks>
        ///     This does not scale. If that is required, use an implementation
        ///     of <see cref="GetVlogLikeSummaryForVlogAsync(Guid)"/> which does
        ///     not return all vlog likes but only a subset.
        /// </remarks>
        /// <param name="vlogId">Internal vlog id</param>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>Vlog like collection</returns>
        IAsyncEnumerable<VlogLike> GetVlogLikesForVlogAsync(Guid vlogId, Navigation navigation);

        /// <summary>
        ///     Gets all <see cref="VlogLikingUserWrapper"/> objects that 
        ///     belong to the vlogs of a given <paramref name="userId"/>.
        /// </summary>
        /// <param name="navigation">Result set control.</param>
        /// <returns>Wrappers around all users that liked saids vlogs.</returns>
        IAsyncEnumerable<VlogLikingUserWrapper> GetVlogLikingUsersForUserAsync(Navigation navigation);

        /// <summary>
        ///     Gets a like summery for a given vlog.
        /// </summary>
        /// <param name="vlogId">Internal vlog id</param>
        /// <returns>Vlog like summary.</returns>
        Task<VlogLikeSummary> GetVlogLikeSummaryForVlogAsync(Guid vlogId);
        
        /// <summary>
        ///     Used when the current user likes a vlog.
        /// </summary>
        /// <param name="vlogId">The vlog to like.</param>
        Task LikeAsync(Guid vlogId);

        /// <summary>
        ///     Used when the current user unlikes a vlog.
        /// </summary>
        /// <param name="vlogId">The vlog to unlike.</param>
        Task UnlikeAsync(Guid vlogId);
    }
}
