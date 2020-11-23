using Swabbr.Core.Entities;
using Swabbr.Core.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.Services
{
    /// <summary>
    ///     Contract for a service that handles everything 
    ///     related to <see cref="Reaction"/> entities.
    /// </summary>
    public interface IReactionService
    {        
        /// <summary>
        ///     Soft deletes a reaction in our data store.
        /// </summary>
        /// <param name="reactionId">The reaction to be deleted.</param>
        Task DeleteReactionAsync(Guid reactionId);

        /// <summary>
        ///     Gets a reaction from our data store.
        /// </summary>
        /// <param name="reactionId">The reaction id.</param>
        /// <returns>The reaction.</returns>
        Task<Reaction> GetAsync(Guid reactionId);

        /// <summary>
        ///     Gets the amount of reactions for a vlog.
        /// </summary>
        /// <param name="vlogId">The vlog id.</param>
        /// <returns>The amount of reactions.</returns>
        Task<uint> GetReactionCountForVlogAsync(Guid vlogId);

        /// <summary>
        ///     Gets all reaction for a vlog.
        /// </summary>
        /// <param name="vlogId">The vlog of the reactions.</param>
        /// <returns>All vlog reactions.</returns>
        Task<IEnumerable<Reaction>> GetReactionsForVlogAsync(Guid vlogId);

        /// <summary>
        ///     Gets all reactions for a vlog including
        ///     their thumbnail details.
        /// </summary>
        /// <param name="vlogId">The vlog of the reactions.</param>
        /// <returns>All vlog reactions with their thumbnails.</returns>
        Task<IEnumerable<ReactionWithThumbnailDetails>> GetReactionsForVlogWithThumbnailsAsync(Guid vlogId);

        /// <summary>
        ///     Gets a reaction including its thumbnail details.
        /// </summary>
        /// <param name="reactionId">The reaction id.</param>
        /// <returns>The reaction with thumbnail details.</returns>
        Task<ReactionWithThumbnailDetails> GetWithThumbnailAsync(Guid reactionId);

        // TODO Also return reaction?
        /// <summary>
        ///     Called when a reaction has been uploaded. This will
        ///     actually post the reaction.
        /// </summary>
        /// <param name="targetVlogId">The vlog the reaction was posted to.</param>
        /// <param name="reactionId">The uploaded reaction id.</param>
        Task PostReactionAsync(Guid targetVlogId, Guid reactionId);

        /// <summary>
        ///     Updates a reaction in our data store.
        /// </summary>
        /// <param name="reaction">The reaction with updated properties.</param>
        Task UpdateReactionAsync(Reaction reaction);
    }
}
