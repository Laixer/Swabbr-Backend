﻿using Swabbr.Core.Context;
using Swabbr.Core.Entities;
using Swabbr.Core.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.Services
{
    /// <summary>
    ///     Service for processing vlog and vlog like operations.
    /// </summary>
    /// <remarks>
    ///     The executing user id is never passed. Whenever 
    ///     possible, this id is extracted from the context.
    /// </remarks>
    public interface IVlogService
    {
        /// <summary>
        ///     Adds views for given vlogs.
        /// </summary>
        /// <param name="context">Context for adding vlog views.</param>
        Task AddViews(AddVlogViewsContext context);

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
        ///     Generates an upload uri for a new vlog.
        /// </summary>
        /// <param name="vlogId">The new vlog id.</param>
        /// <returns>Upload details.</returns>
        Task<UploadWrapper> GenerateUploadUri(Guid vlogId);

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
        ///     Gets vlogs that belong to a user.
        /// </summary>
        /// <param name="userId">The vlog owner.</param>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>Vlog collection.</returns>
        IAsyncEnumerable<Vlog> GetVlogsByUserAsync(Guid userId, Navigation navigation);

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
        ///     Used when the current user likes a vlog.
        /// </summary>
        /// <param name="vlogId">The vlog to like.</param>
        Task LikeAsync(Guid vlogId);

        /// <summary>
        ///     Called when a vlog has been uploaded to the blob 
        ///     storage and the user wishes to publish it.
        /// </summary>
        /// <param name="context">Context for posting a vlog.</param>
        Task PostVlogAsync(PostVlogContext context);

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
