using Swabbr.Core.Entities;
using Swabbr.Core.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.Services
{

    /// <summary>
    /// Contract for a service that handles everything related to <see cref="Reaction"/> entities.
    /// </summary>
    public interface IReactionService
    {

        Task DeleteReactionAsync(Guid userId, Guid reactionId);

        /// <summary>
        /// Gets a new uploading uri for a <see cref="Reaction"/> upload.
        /// </summary>
        /// <param name="userId">Internal <see cref="SwabbrUser"/> id</param>
        /// <param name="reactionId">Internal <see cref="Reaction"/> id</param>
        /// <returns><see cref="Uri"/></returns>
        Task<Uri> GetNewUploadUriAsync(Guid userId, Guid reactionId);

        Task<SwabbrUser> GetOwnerOfVlogByReactionAsync(Guid reactionId);

        Task<Reaction> GetReactionAsync(Guid reactionId);

        Task<int> GetReactionCountForVlogAsync(Guid vlogId);

        Task<IEnumerable<Reaction>> GetReactionsForVlogAsync(Guid vlogId);

        /// <summary>
        /// Called when a <see cref="Reaction"/> is uploaded.
        /// </summary>
        /// <param name="reactionId">Internal <see cref="Reaction"/> id</param>
        /// <returns><see cref="Task"/></returns>
        Task OnFinishedUploadingReactionAsync(Guid reactionId);

        Task OnTranscodingReactionFailedAsync(Guid reactionId);

        Task OnTranscodingReactionSucceededAsync(Guid reactionId);

        /// <summary>
        /// Called when we want to upload a reaction.
        /// </summary>
        /// <param name="userId">Internal <see cref="SwabbrUser"/> id</param>
        /// <param name="targetVlogId">Internal <see cref="Vlog"/> id</param>
        /// <param name="isPrivate">Indicates vlog private or not</param>
        /// <returns><see cref="ReactionUploadWrapper"/></returns>
        Task<ReactionUploadWrapper> PostReactionAsync(Guid userId, Guid targetVlogId, bool isPrivate);

        Task<Reaction> UpdateReactionAsync(Guid userId, Guid reactionId, bool isPrivate);

    }

}
