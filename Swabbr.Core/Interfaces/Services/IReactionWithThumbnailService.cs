using Swabbr.Core.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.Services
{
    /// <summary>
    ///     Contract for retrieving reactions with their thumbnails.
    /// </summary>
    public interface IReactionWithThumbnailService : IReactionService
    {
        /// <summary>
        ///     Gets a <see cref="Entities.Reaction"/> with its corresponding 
        ///     thumbnail details.
        /// </summary>
        /// <param name="reactionId">Internal <see cref="Entities.Reaction"/> id</param>
        /// <returns><see cref="VlogWithThumbnailDetails"/></returns>
        Task<ReactionWithThumbnailDetails> GetWithThumbnailDetailsAsync(Guid reactionId);

        /// <summary>
        ///     Gets all <see cref="Entities.Reaction"/> with its corresponding 
        ///     thumbnail details for a given <paramref name="vlogId"/>.
        /// </summary>
        /// <param name="vlogId">Internal <see cref="Entities.Vlog"/> id</param>
        /// <returns><see cref="VlogWithThumbnailDetails"/></returns>
        Task<IEnumerable<ReactionWithThumbnailDetails>> GetWithThumbnailForVlogAsync(Guid vlogId);
    }
}
