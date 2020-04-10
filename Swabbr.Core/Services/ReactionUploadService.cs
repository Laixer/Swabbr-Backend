using Laixer.Utility.Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Swabbr.Core.Entities;
using Swabbr.Core.Enums;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Interfaces.Services;
using Swabbr.Core.Types;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Transactions;

namespace Swabbr.Core.Services
{

    /// <summary>
    /// Service for uploading reactions.
    /// </summary>
    public sealed class ReactionUploadService : IReactionUploadService
    {

        private readonly IReactionRepository _reactionRepository;
        private readonly ITranscodingService _transcodingService;
        private readonly IStorageService _storageService;
        private readonly INotificationService _notificationService;

        private readonly ILogger logger;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public ReactionUploadService(IReactionRepository reactionRepository,
            ITranscodingService transcodingService,
            IStorageService storageService,
            INotificationService notificationService,
            ILoggerFactory loggerFactory)
        {
            _reactionRepository = reactionRepository ?? throw new ArgumentNullException(nameof(reactionRepository));
            _transcodingService = transcodingService ?? throw new ArgumentNullException(nameof(transcodingService));
            _storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            logger = (loggerFactory != null) ? loggerFactory.CreateLogger(nameof(ReactionUploadService)) : throw new ArgumentNullException(nameof(loggerFactory));
        }

        /// <summary>
        /// This creates a new reaction upload stream. The reaction is not yet
        /// created in our data store, in case of potential upload failure.
        /// </summary>
        /// <param name="userId">Internal <see cref="SwabbrUser"/> id</param>
        /// <param name="targetVlogId">Internal <see cref="Vlog"/> id</param>
        /// <returns><see cref="Stream"/></returns>
        public async Task<StreamWithEntityIdWrapper> GetReactionUploadStreamAsync(Guid userId, Guid targetVlogId)
        {
            userId.ThrowIfNullOrEmpty();
            targetVlogId.ThrowIfNullOrEmpty();

            var reaction = await _reactionRepository.CreateAsync(new Reaction
            {
                UserId = userId,
                TargetVlogId = targetVlogId,
            }).ConfigureAwait(false);

            return await _transcodingService.GetStreamForReactionUploadAsync(reaction.Id).ConfigureAwait(false);
        }

        /// <summary>
        /// This should be called when we are done uploading a reaction. This 
        /// will launch a new job in our transcoding service.
        /// </summary>
        /// <remarks>
        /// This does NOT await job completion.
        /// </remarks>
        /// <param name="userId">Internal <see cref="SwabbrUser"/> id</param>
        /// <param name="reactionId">Internal <see cref="Reaction"/> id</param>
        /// <returns><see cref="Task"/></returns>
        public async Task OnFinishedUploadingReactionAsync(Guid userId, Guid reactionId)
        {
            userId.ThrowIfNullOrEmpty();
            reactionId.ThrowIfNullOrEmpty();

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var reaction = await _reactionRepository.GetAsync(reactionId).ConfigureAwait(false);
                if (reaction.UserId != userId) { throw new UserNotOwnerException(nameof(Reaction)); }
                if (reaction.ReactionProcessingState != ReactionProcessingState.Created) { throw new InvalidOperationException($"Reaction was not in state {ReactionProcessingState.Created.GetEnumMemberAttribute()}"); }
                
                await _reactionRepository.UpdateStatusAsync(reactionId, ReactionProcessingState.Processing).ConfigureAwait(false);
                scope.Complete();
            }

            await _transcodingService.ProcessReactionAsync(reactionId).ConfigureAwait(false);
        }

        /// <summary>
        /// Should be triggered when a transcoding process finishes transcoding a reaction.
        /// </summary>
        /// <param name="reactionId">Internal <see cref="Reaction"/> id</param>
        /// <returns><see cref="Task"/></returns>
        public async Task OnFinishedTranscodingReactionAsync(Guid reactionId)
        {
            reactionId.ThrowIfNullOrEmpty();

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var reaction = await _reactionRepository.GetAsync(reactionId).ConfigureAwait(false);

                if (reaction.ReactionProcessingState != ReactionProcessingState.Processing) { throw new InvalidOperationException($"Reaction was not in state {ReactionProcessingState.Processing.GetEnumMemberAttribute()}"); }

                // Delete input asset, job and CDN container
                await _storageService.CleanupReactionStorageAsync(reactionId).ConfigureAwait(false);

                // Extract metadata
                reaction.LengthInSeconds = await _transcodingService.ExtractVideoLengthInSecondsAsync(reactionId).ConfigureAwait(false);

                // Update, mark status and commit (TODO Maybe one call after all?)
                await _reactionRepository.UpdateAsync(reaction).ConfigureAwait(false);
                await _reactionRepository.UpdateStatusAsync(reactionId, ReactionProcessingState.Finished).ConfigureAwait(false);
                scope.Complete();
            }

            await _notificationService.NotifyReactionPlacedAsync(reactionId).ConfigureAwait(false);
        }

        /// <summary>
        /// Should be called when a reaction transcoding process fails.
        /// </summary>
        /// <param name="reactionId">Internal <see cref="Reaction"/> id</param>
        /// <returns><see cref="Task"/></returns>
        public async Task OnFailedTranscodingReactionAsync(Guid reactionId)
        {
            reactionId.ThrowIfNullOrEmpty();

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var reaction = await _reactionRepository.GetAsync(reactionId).ConfigureAwait(false);
                if (reaction.ReactionProcessingState != ReactionProcessingState.Processing) { throw new InvalidOperationException($"Reaction was not in state {ReactionProcessingState.Processing.GetEnumMemberAttribute()}"); }

                // TODO Handle

                // Mark status and commit
                await _reactionRepository.UpdateStatusAsync(reactionId, ReactionProcessingState.Failed).ConfigureAwait(false);
                scope.Complete();
            }

            // TODO Notify the user that a reaction failed to process?
        }

    }

}
