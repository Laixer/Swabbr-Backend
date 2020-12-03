﻿using Swabbr.Core.Abstractions;
using Swabbr.Core.Entities;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Interfaces.Services;
using Swabbr.Core.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swabbr.Core.Services
{
    // TODO Implement.
    /// <summary>
    ///     Reaction related operations.
    /// </summary>
    public class ReactionService : AppServiceBase, IReactionService
    {
        protected readonly INotificationService _notificationService;
        protected readonly IReactionRepository _reactionRepository;
        protected readonly IVlogRepository _vlogRepository;

        /// <summary>
        ///     Create new instance.
        /// </summary>
        public ReactionService(AppContext appContext,
            INotificationService notificationService, 
            IReactionRepository reactionRepository,
            IVlogRepository vlogRepository)
        {
            // TODO Is this correct usage? Not sure, since this implementation doesn't guarantee to be scoped.
            AppContext = appContext ?? throw new ArgumentNullException();
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _reactionRepository = reactionRepository ?? throw new ArgumentNullException(nameof(reactionRepository));
            _vlogRepository = vlogRepository ?? throw new ArgumentNullException(nameof(vlogRepository));
        }

        /// <summary>
        ///     Soft deletes a reaction in our data store.
        /// </summary>
        /// <param name="reactionId">The reaction to be deleted.</param>
        public Task DeleteReactionAsync(Guid reactionId)
            => _reactionRepository.DeleteAsync(reactionId);

        /// <summary>
        ///     Gets a reaction from our data store.
        /// </summary>
        /// <param name="reactionId">The reaction id.</param>
        /// <returns>The reaction.</returns>
        public Task<Reaction> GetAsync(Guid reactionId)
            => _reactionRepository.GetAsync(reactionId);

        /// <summary>
        ///     Gets the amount of reactions for a vlog.
        /// </summary>
        /// <param name="vlogId">The vlog id.</param>
        /// <returns>The amount of reactions.</returns>
        public Task<uint> GetReactionCountForVlogAsync(Guid vlogId)
            => _reactionRepository.GetCountForVlogAsync(vlogId);

        /// <summary>
        ///     Gets all reaction for a vlog.
        /// </summary>
        /// <param name="vlogId">The vlog of the reactions.</param>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>All vlog reactions.</returns>
        public IAsyncEnumerable<Reaction> GetReactionsForVlogAsync(Guid vlogId, Navigation navigation)
            => _reactionRepository.GetForVlogAsync(vlogId, navigation);

        /// <summary>
        ///     Gets all reactions for a vlog including
        ///     their thumbnail details.
        /// </summary>
        /// <param name="vlogId">The vlog of the reactions.</param>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>All vlog reactions with their thumbnails.</returns>
        public async IAsyncEnumerable<ReactionWithThumbnailDetails> GetReactionsForVlogWithThumbnailsAsync(Guid vlogId, Navigation navigation)
        {
            await foreach (var reaction in GetReactionsForVlogAsync(vlogId, navigation))
            {
                yield return new ReactionWithThumbnailDetails
                {
                    Reaction = reaction,
                    ThumbnailUri = null // TODO
                };
            }
        }

        /// <summary>
        ///     Gets a reaction including its thumbnail details.
        /// </summary>
        /// <param name="reactionId">The reaction id.</param>
        /// <returns>The reaction with thumbnail details.</returns>
        public async Task<ReactionWithThumbnailDetails> GetWithThumbnailAsync(Guid reactionId)
            => new ReactionWithThumbnailDetails
            {
                Reaction = await GetAsync(reactionId),
                ThumbnailUri = null // TODO
            };

        /// <summary>
        ///     Called when a reaction has been uploaded. This will
        ///     actually post the reaction.
        /// </summary>
        /// <param name="targetVlogId">The vlog the reaction was posted to.</param>
        /// <param name="reactionId">The uploaded reaction id.</param>
        public async Task PostReactionAsync(Guid targetVlogId, Guid reactionId)
        {
            // This can never happen if we aren't logged in.
            if (!AppContext.HasUser)
            {
                throw new InvalidOperationException();
            }

            var reaction = new Reaction
            {
                Id = reactionId,
                TargetVlogId = targetVlogId,
                UserId = AppContext.UserId
            };

            await _reactionRepository.CreateAsync(reaction);

            var targetVlog = await _vlogRepository.GetAsync(targetVlogId);
            await _notificationService.NotifyReactionPlacedAsync(targetVlog.UserId, targetVlogId, reactionId);                
        }

        // TODO Look at this
        /// <summary>
        ///     Updates a reaction in our data store.
        /// </summary>
        /// <param name="reaction">The reaction with updated properties.</param>
        public Task UpdateReactionAsync(Reaction reaction)
            => _reactionRepository.UpdateAsync(reaction);

    }
}
