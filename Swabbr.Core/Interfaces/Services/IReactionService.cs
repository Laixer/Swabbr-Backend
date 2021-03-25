using Swabbr.Core.Context;
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
    /// <remarks>
    ///     The executing user id is never passed. Whenever possible,
    ///     this id is extracted from the context.
    /// </remarks>
    public interface IReactionService
    {        
        /// <summary>
        ///     Soft deletes a reaction in our data store.
        /// </summary>
        /// <param name="reactionId">The reaction to be deleted.</param>
        Task DeleteReactionAsync(Guid reactionId);

        /// <summary>
        ///     Generates an upload uri for a new reaction.
        /// </summary>
        /// <param name="reactionId">The new reaction id.</param>
        /// <returns>Upload wrapper.</returns>
        Task<UploadWrapper> GenerateUploadDetails(Guid reactionId);

        /// <summary>
        ///     Gets a reaction from our data store.
        /// </summary>
        /// <param name="reactionId">The reaction id.</param>
        /// <returns>The reaction.</returns>
        Task<Reaction> GetAsync(Guid reactionId);

        /// <summary>
        ///     Gets a reaction wrapper by id.
        /// </summary>
        /// <param name="id">The internal reaction id.</param>
        /// <returns>Reaction wrapper.</returns>
        Task<ReactionWrapper> GetWrapperAsync(Guid id);

        /// <summary>
        ///     Gets the amount of reactions for a vlog.
        /// </summary>
        /// <param name="vlogId">The vlog id.</param>
        /// <returns>The amount of reactions.</returns>
        Task<uint> GetCountForVlogAsync(Guid vlogId);

        /// <summary>
        ///     Gets all reaction for a vlog.
        /// </summary>
        /// <param name="vlogId">The vlog of the reactions.</param>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>All vlog reactions.</returns>
        IAsyncEnumerable<Reaction> GetForVlogAsync(Guid vlogId, Navigation navigation);

        /// <summary>
        ///     Gets reaction wrappers that belong to a vlog.
        /// </summary>
        /// <param name="vlogId">The corresponding vlog.</param>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>Reaction wrappers for the vlog.</returns>
        IAsyncEnumerable<ReactionWrapper> GetWrappersForVlogAsync(Guid vlogId, Navigation navigation);

        /// <summary>
        ///     Called when a reaction has been uploaded. This will
        ///     actually post the reaction.
        /// </summary>
        /// <param name="context">Context for posting a reaction.</param>
        Task PostAsync(PostReactionContext context);

        /// <summary>
        ///     Updates a reaction in our data store.
        /// </summary>
        /// <param name="reaction">The reaction with updated properties.</param>
        Task UpdateAsync(Reaction reaction);
    }
}
