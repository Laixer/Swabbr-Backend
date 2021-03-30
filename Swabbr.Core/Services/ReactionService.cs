using Swabbr.Core.Abstractions;
using Swabbr.Core.Context;
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
        private readonly INotificationService _notificationService;
        private readonly IBlobStorageService _blobStorageService;
        private readonly IEntityStorageUriService _entityStorageUriService;
        private readonly IReactionRepository _reactionRepository;
        private readonly IVlogRepository _vlogRepository;

        /// <summary>
        ///     Create new instance.
        /// </summary>
        public ReactionService(INotificationService notificationService, 
            IBlobStorageService blobStorageService,
            IEntityStorageUriService entityStorageUriService,
            IReactionRepository reactionRepository,
            IVlogRepository vlogRepository)
        {
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _blobStorageService = blobStorageService ?? throw new ArgumentNullException(nameof(blobStorageService));
            _entityStorageUriService = entityStorageUriService ?? throw new ArgumentNullException(nameof(entityStorageUriService));
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
        public Task DeleteReactionAsync(Guid reactionId)
            => _reactionRepository.DeleteAsync(reactionId);

        // TODO Hardcoded content type.
        /// <summary>
        ///     Generates an upload uri for a new reaction.
        /// </summary>
        /// <param name="reactionId">The new reaction id.</param>
        /// <returns>Upload uri.</returns>
        public async Task<UploadWrapper> GenerateUploadDetails(Guid reactionId)
            => new UploadWrapper
            {
                Id = reactionId,
                ThumbnailUploadUri = await _blobStorageService.GenerateUploadLinkAsync(
                    containerName: StorageConstants.ReactionStorageFolderName,
                    fileName: StorageHelper.GetThumbnailFileName(reactionId),
                    timeSpanValid: TimeSpan.FromHours(2),
                    contentType: "image/jpeg"),
                VideoUploadUri = await _blobStorageService.GenerateUploadLinkAsync(
                    containerName: StorageConstants.ReactionStorageFolderName,
                    fileName: StorageHelper.GetVideoFileName(reactionId),
                    timeSpanValid: TimeSpan.FromHours(2),
                    contentType: "video/mp4")
            };

        /// <summary>
        ///     Gets a reaction from our data store.
        /// </summary>
        /// <param name="reactionId">The reaction id.</param>
        /// <returns>The reaction.</returns>
        public async Task<Reaction> GetAsync(Guid reactionId)
        {
            var reaction = await _reactionRepository.GetAsync(reactionId);

            reaction.ThumbnailUri = await _entityStorageUriService.GetReactionThumbnailAccessUriAsync(reaction.Id);
            reaction.VideoUri = await _entityStorageUriService.GetReactionVideoAccessUriAsync(reaction.Id);

            return reaction;
        }

        /// <summary>
        ///     Gets the amount of reactions for a vlog.
        /// </summary>
        /// <param name="vlogId">The vlog id.</param>
        /// <returns>The amount of reactions.</returns>
        public Task<uint> GetCountForVlogAsync(Guid vlogId)
            => _reactionRepository.GetCountForVlogAsync(vlogId);

        /// <summary>
        ///     Gets all reaction for a vlog.
        /// </summary>
        /// <param name="vlogId">The vlog of the reactions.</param>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>All vlog reactions.</returns>
        public async IAsyncEnumerable<Reaction> GetForVlogAsync(Guid vlogId, Navigation navigation)
        {
            await foreach (var reaction in _reactionRepository.GetForVlogAsync(vlogId, navigation))
            {
                reaction.ThumbnailUri = await _entityStorageUriService.GetReactionThumbnailAccessUriAsync(reaction.Id);
                reaction.VideoUri = await _entityStorageUriService.GetReactionVideoAccessUriAsync(reaction.Id);

                yield return reaction;
            }
        }

        /// <summary>
        ///     Get a reaction wrapper from our data store.
        /// </summary>
        /// <param name="id">Reaction id.</param>
        /// <returns>Reaction wrapper.</returns>
        public async Task<ReactionWrapper> GetWrapperAsync(Guid id)
        {
            var reactionWrapper = await _reactionRepository.GetWrapperAsync(id);

            reactionWrapper.Reaction.ThumbnailUri = await _entityStorageUriService.GetReactionThumbnailAccessUriAsync(reactionWrapper.Reaction.Id);
            reactionWrapper.Reaction.VideoUri = await _entityStorageUriService.GetReactionVideoAccessUriAsync(reactionWrapper.Reaction.Id);
            reactionWrapper.User.ProfileImageUri = await _entityStorageUriService.GetUserProfileImageAccessUriOrNullAsync(reactionWrapper.User);

            return reactionWrapper;
        }

        /// <summary>
        ///     Get all reaction wrappers for a vlog from our data store.
        /// </summary>
        /// <param name="vlogId">The vlog id.</param>
        /// <param name="navigation">Result set control.</param>
        /// <returns>Reaction wrappers for vlog.</returns>
        public async IAsyncEnumerable<ReactionWrapper> GetWrappersForVlogAsync(Guid vlogId, Navigation navigation)
        {
            await foreach (var reactionWrapper in _reactionRepository.GetWrappersForVlogAsync(vlogId, navigation))
            {
                reactionWrapper.Reaction.ThumbnailUri = await _entityStorageUriService.GetReactionThumbnailAccessUriAsync(reactionWrapper.Reaction.Id);
                reactionWrapper.Reaction.VideoUri = await _entityStorageUriService.GetReactionVideoAccessUriAsync(reactionWrapper.Reaction.Id);
                reactionWrapper.User.ProfileImageUri = await _entityStorageUriService.GetUserProfileImageAccessUriOrNullAsync(reactionWrapper.User);

                yield return reactionWrapper;
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
        /// <param name="context">Context for posting a reaction.</param>
        public async Task PostAsync(PostReactionContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (!await _blobStorageService.FileExistsAsync(StorageConstants.ReactionStorageFolderName, context.ReactionId.ToString())) 
            {
                throw new FileNotFoundException();
            }

            var reaction = new Reaction
            {
                Id = context.ReactionId,
                TargetVlogId = context.TargetVlogId,
                UserId = context.UserId,
            };

            await _reactionRepository.CreateAsync(reaction);

            var targetVlog = await _vlogRepository.GetAsync(context.TargetVlogId);

            await _notificationService.NotifyReactionPlacedAsync(targetVlog.UserId, context.TargetVlogId, context.ReactionId);
        }

        /// <summary>
        ///     Updates a reaction in our data store.
        /// </summary>
        /// <remarks>
        ///     This expects the current user to own the reaction.
        /// </remarks>
        /// <param name="reaction">The reaction with updated properties.</param>
        public Task UpdateAsync(Reaction reaction)
            => _reactionRepository.UpdateAsync(reaction);
    }
}
