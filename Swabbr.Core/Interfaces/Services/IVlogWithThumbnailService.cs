using Swabbr.Core.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.Services
{
    /// <summary>
    ///     Contract for retrieving vlogs with their thumbnails.
    /// </summary>
    public interface IVlogWithThumbnailService : IVlogService
    {
        /// <summary>
        ///     Gets a <see cref="Entities.Vlog"/> with its corresponding 
        ///     thumbnail details.
        /// </summary>
        /// <param name="vlogId">Internal <see cref="Entities.Vlog"/> id</param>
        /// <returns><see cref="VlogWithThumbnailDetails"/></returns>
        Task<VlogWithThumbnailDetails> GetWithThumbnailDetailsAsync(Guid vlogId);

        /// <summary>
        ///     Gets all <see cref="Entities.Vlog"/>s for a given <see cref="Entities.SwabbrUser"/>.
        /// </summary>
        /// <param name="userId">Internal <see cref="Entities.SwabbrUser"/> id</param>
        /// <returns><see cref="VlogWithThumbnailDetails"/> collection</returns>
        Task<IEnumerable<VlogWithThumbnailDetails>> GetVlogsWithThumbnailDetailsFromUserAsync(Guid userId);

        /// <summary>
        ///     Gets all recommended <see cref="Entities.Vlog"/>s for a given <see cref="Entities.SwabbrUser"/>.
        /// </summary>
        /// <param name="userId">Internal <see cref="Entities.SwabbrUser"/> id</param>
        /// <param name="maxCount">Max result set size</param>
        /// <returns><see cref="VlogWithThumbnailDetails"/> collection</returns>
        Task<IEnumerable<VlogWithThumbnailDetails>> GetRecommendedVlogsWithThumbnailDetailsForUserAsync(Guid userId, uint maxCount);
    }
}
