using Swabbr.Core.Entities;
using Swabbr.Core.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.Services
{
    /// <summary>
    /// Service for processing vlog and vlog like operations.
    /// </summary>
    /// <remarks>
    ///     The executing user id is never passed. Whenever 
    ///     possible, this id is extracted from the context.
    /// </remarks>
    public interface IVlogService
    {
        /// <summary>
        ///     Adds a view to a vlog.
        /// </summary>
        /// <param name="vlogId">The vlog that is watched.</param>
        Task AddView(Guid vlogId);

        /// <summary>
        ///     Soft deletes a vlog in our data store.
        /// </summary>
        /// <param name="vlogId">The vlog to delete.</param>
        Task DeleteAsync(Guid vlogId);

        /// <summary>
        ///     Checks if a vlog exists in our data store.
        /// </summary>
        /// <param name="vlogId">The vlog id to check.</param>
        Task<bool> ExistsAsync(Guid vlogId);

        /// <summary>
        ///     Gets a vlog from our data store.
        /// </summary>
        /// <param name="vlogId">The vlog id.</param>
        /// <returns>The vlog.</returns>
        Task<Vlog> GetAsync(Guid vlogId);

        /// <summary>
        ///     Gets recommended vlogs for the current user.
        /// </summary>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>Recommended vlogs.</returns>
        IAsyncEnumerable<Vlog> GetRecommendedForUserAsync(Navigation navigation);

        /// <summary>
        ///     Gets recommended vlogs for the current user 
        ///     including their thumbnail details.
        /// </summary>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>Vlogs with thumbnail details.</returns>
        IAsyncEnumerable<VlogWithThumbnailDetails> GetRecommendedForUserWithThumbnailsAsync(Navigation navigation);

        /// <summary>
        ///     Gets vlogs that belong to a user.
        /// </summary>
        /// <param name="userId">The vlog owner.</param>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>Vlog collection.</returns>
        IAsyncEnumerable<Vlog> GetVlogsByUserAsync(Guid userId, Navigation navigation);

        /// <summary>
        ///     Gets vlogs that belong to a user including
        ///     their thumbnail details.
        /// </summary>
        /// <param name="userId">The corresponding user.</param>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>All vlogs belonging to the user.</returns>
        IAsyncEnumerable<VlogWithThumbnailDetails> GetVlogsByUserWithThumbnailsAsync(Guid userId, Navigation navigation);

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
        ///     Gets a like summery for a given vlog.
        /// </summary>
        /// <param name="vlogId">Internal vlog id</param>
        /// <returns>Vlog like summary.</returns>
        Task<VlogLikeSummary> GetVlogLikeSummaryForVlogAsync(Guid vlogId);

        /// <summary>
        ///     Gets a vlog including its thumbnail details.
        /// </summary>
        /// <param name="vlogId">The vlog id.</param>
        /// <returns>Vlog with thumbnail details.</returns>
        Task<VlogWithThumbnailDetails> GetWithThumbnailAsync(Guid vlogId);
        
        /// <summary>
        ///     Used when the current user likes a vlog.
        /// </summary>
        /// <param name="vlogId">The vlog to like.</param>
        Task LikeAsync(Guid vlogId);

        /// <summary>
        ///     Called when a vlog has been uploaded to the blob 
        ///     storage and the user wishes to publish it.
        /// </summary>
        /// <param name="vlogId">The uploaded vlog id.</param>
        /// <param name="isPrivate">Accessibility of the vlog.</param>
        Task PostVlogAsync(Guid vlogId, bool isPrivate);

        /// <summary>
        ///     Used when the current user unlikes a vlog.
        /// </summary>
        /// <param name="vlogId">The vlog to unlike.</param>
        Task UnlikeAsync(Guid vlogId);

        /// <summary>
        ///     Updates a vlog in our data store.
        /// </summary>
        /// <param name="vlog">The vlog with updates properties.</param>
        Task UpdateAsync(Vlog vlog);
    }
}
