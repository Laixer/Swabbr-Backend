using Swabbr.Core.Abstractions;
using Swabbr.Core.Entities;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Interfaces.Services;
using Swabbr.Core.Storage;
using Swabbr.Core.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swabbr.Core.Services
{
    /// <summary>
    ///     Reaction related operations.
    /// </summary>
    public class ReactionService : AppServiceBase, IReactionService
    {
        protected readonly INotificationService _notificationService;
        protected readonly IBlobStorageService _blobStorageService;
        protected readonly IReactionRepository _reactionRepository;
        protected readonly IVlogRepository _vlogRepository;

        /// <summary>
        ///     Create new instance.
        /// </summary>
        public ReactionService(INotificationService notificationService, 
            IBlobStorageService blobStorageService,
            IReactionRepository reactionRepository,
            IVlogRepository vlogRepository)
        {
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _blobStorageService = blobStorageService ?? throw new ArgumentNullException(nameof(blobStorageService));
            _reactionRepository = reactionRepository ?? throw new ArgumentNullException(nameof(reactionRepository));
            _vlogRepository = vlogRepository ?? throw new ArgumentNullException(nameof(vlogRepository));
        }

        /// <summary>
        ///     Soft deletes a reaction in our data store.
        /// </summary>
        /// <remarks>
        ///     This expects the current user to own the reaction.
        /// </remarks>
        /// <param name="reactionId">The reaction to be deleted.</param>
        public virtual Task DeleteReactionAsync(Guid reactionId)
            => _reactionRepository.DeleteAsync(reactionId);

        /// <summary>
        ///     Gets a reaction from our data store.
        /// </summary>
        /// <param name="reactionId">The reaction id.</param>
        /// <returns>The reaction.</returns>
        public async virtual Task<Reaction> GetAsync(Guid reactionId)
        {
            var reaction = await _reactionRepository.GetAsync(reactionId);

            reaction.ThumbnailUri = await GetThumbnailUriAsync(reaction);
            reaction.VideoUri = await GetVideoUriAsync(reaction);

            return reaction;
        }

        /// <summary>
        ///     Gets the amount of reactions for a vlog.
        /// </summary>
        /// <param name="vlogId">The vlog id.</param>
        /// <returns>The amount of reactions.</returns>
        public virtual Task<uint> GetReactionCountForVlogAsync(Guid vlogId)
            => _reactionRepository.GetCountForVlogAsync(vlogId);

        /// <summary>
        ///     Gets all reaction for a vlog.
        /// </summary>
        /// <param name="vlogId">The vlog of the reactions.</param>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>All vlog reactions.</returns>
        public async virtual IAsyncEnumerable<Reaction> GetReactionsForVlogAsync(Guid vlogId, Navigation navigation)
        {
            await foreach (var reaction in _reactionRepository.GetForVlogAsync(vlogId, navigation))
            {
                reaction.ThumbnailUri = await GetThumbnailUriAsync(reaction);
                reaction.VideoUri = await GetVideoUriAsync(reaction);

                yield return reaction;
            }
        }

        /// <summary>
        ///     Called when a reaction has been uploaded. This will
        ///     post the reaction and notify the vlog owner.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         The reaction will belong to the current user.
        ///     </para>
        ///     <para>
        ///         If the video file or thumbnail file does not 
        ///         exist in our blob storage this throws a 
        ///         <see cref="FileNotFoundException"/>.
        ///     </para>
        /// </remarks>
        /// <param name="targetVlogId">The vlog the reaction was posted to.</param>
        /// <param name="reactionId">The uploaded reaction id.</param>
        public virtual async Task PostReactionAsync(Guid targetVlogId, Guid reactionId)
        {
            if (!await _blobStorageService.FileExistsAsync(StorageConstants.ReactionStorageFolderName, reactionId.ToString())) 
            {
                throw new FileNotFoundException();
            }

            var reaction = new Reaction
            {
                Id = reactionId,
                TargetVlogId = targetVlogId,
            };

            // Note: The user id is assigned by the reaction repository based on the context.
            // TODO This comment could not have been made without full knowledge of the repo, which we can't always have!
            await _reactionRepository.CreateAsync(reaction);

            var targetVlog = await _vlogRepository.GetAsync(targetVlogId);
            // FUTURE: Enqueue
            await _notificationService.NotifyReactionPlacedAsync(targetVlog.UserId, targetVlogId, reactionId);                
        }

        /// <summary>
        ///     Updates a reaction in our data store.
        /// </summary>
        /// <remarks>
        ///     This expects the current user to own the reaction.
        /// </remarks>
        /// <param name="reaction">The reaction with updated properties.</param>
        public virtual Task UpdateReactionAsync(Reaction reaction)
            => _reactionRepository.UpdateAsync(reaction);

        /// <summary>
        ///     Extract the thumbnail uri for a reaction.
        /// </summary>
        /// <param name="reaction">The reaction.</param>
        /// <returns>Thumbnail uri.</returns>
        private Task<Uri> GetThumbnailUriAsync(Reaction reaction)
            => _blobStorageService.GetAccessLinkAsync(StorageConstants.ReactionStorageFolderName, StorageHelper.GetThumbnailFileName(reaction.Id), 2);

        /// <summary>
        ///     Extract the video uri for a reaction.
        /// </summary>
        /// <param name="reaction">The reaction.</param>
        /// <returns>Video uri.</returns>
        private Task<Uri> GetVideoUriAsync(Reaction reaction)
            => _blobStorageService.GetAccessLinkAsync(StorageConstants.ReactionStorageFolderName, StorageHelper.GetVideoFileName(reaction.Id), 2);
    }
}
