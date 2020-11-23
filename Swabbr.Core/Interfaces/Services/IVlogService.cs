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
        ///     Gets all recommended vlogs for a user.
        /// </summary>
        /// <param name="userId">The corresponding user.</param>
        /// <param name="maxCount">Maximum result set size.</param>
        /// <returns>Recommended vlogs.</returns>
        Task<IEnumerable<Vlog>> GetRecommendedForUserAsync(Guid userId, uint maxCount);

        /// <summary>
        ///     Gets all recommended vlogs for a user including
        ///     their thumbnail details.
        /// </summary>
        /// <param name="userId">The corresponding user.</param>
        /// <param name="maxCount">Maximum result set count.</param>
        /// <returns>Vlogs with thumbnail details.</returns>
        Task<IEnumerable<VlogWithThumbnailDetails>> GetRecommendedForUserWithThumbnailsAsync(Guid userId, uint maxCount);

        /// <summary>
        ///     Gets all vlogs that belong to a user.
        /// </summary>
        /// <param name="userId">The vlog owner.</param>
        /// <returns>Vlog collection.</returns>
        Task<IEnumerable<Vlog>> GetVlogsFromUserAsync(Guid userId);

        /// <summary>
        ///     Gets all vlogs that belong to a user including
        ///     their thumbnail details.
        /// </summary>
        /// <param name="userId">The corresponding user.</param>
        /// <returns>All vlogs belonging to the user.</returns>
        Task<IEnumerable<VlogWithThumbnailDetails>> GetVlogsFromUserWithThumbnailsAsync(Guid userId);

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
        Task<IEnumerable<VlogLike>> GetVlogLikesForVlogAsync(Guid vlogId);

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

        /// <summary>
        ///     Gets a vlog including its thumbnail details.
        /// </summary>
        /// <param name="vlogId">The vlog id.</param>
        /// <returns>Vlog with thumbnail details.</returns>
        Task<VlogWithThumbnailDetails> GetWithThumbnailAsync(Guid vlogId);
        
        /// <summary>
        ///     Likes a vlog.
        /// </summary>
        /// <param name="vlogId">The vlog to like.</param>
        /// <param name="userId">The user that likes the vlog.</param>
        Task LikeAsync(Guid vlogId, Guid userId);

        /// <summary>
        ///     Called when a vlog has finished uploading.
        /// </summary>
        /// <param name="vlogId">The uploaded vlog id.</param>
        Task PostVlogAsync(Guid vlogId);

        /// <summary>
        ///     Unlikes a vlog.
        /// </summary>
        /// <param name="vlogId">The vlog to unlike.</param>
        /// <param name="userId">The user that unlikes the vlog.</param>
        Task UnlikeAsync(Guid vlogId, Guid userId);

        /// <summary>
        ///     Updates a vlog in our data store.
        /// </summary>
        /// <param name="vlog">The vlog with updates properties.</param>
        Task UpdateAsync(Vlog vlog);
    }
}
