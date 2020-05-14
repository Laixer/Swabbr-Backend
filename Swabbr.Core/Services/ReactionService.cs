//using Laixer.Utility.Extensions;
//using Microsoft.Extensions.Logging;
//using Swabbr.Core.Entities;
//using Swabbr.Core.Exceptions;
//using Swabbr.Core.Interfaces;
//using Swabbr.Core.Interfaces.Repositories;
//using Swabbr.Core.Interfaces.Services;
//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using System.Transactions;

//namespace Swabbr.Core.Services
//{

//    /// <summary>
//    /// Service for everything <see cref="Reaction"/> related.
//    /// </summary>
//    public class ReactionService //: IReactionService
//    {

//        private readonly IReactionRepository _reactionRepository;
//        private readonly IStorageService _storageService;
//        private readonly IVlogRepository _vlogRepository;
//        private readonly IUserRepository _userRepository;
//        private readonly ILogger logger;

//        /// <summary>
//        /// Constructor for dependency injeciton.
//        /// </summary>
//        public ReactionService(IReactionRepository reactionRepository,
//            IStorageService storageService,
//            IVlogRepository vlogRepository,
//            IUserRepository userRepository,
//            ILoggerFactory loggerFactory)
//        {
//            _reactionRepository = reactionRepository ?? throw new ArgumentNullException(nameof(reactionRepository));
//            _storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
//            _vlogRepository = vlogRepository ?? throw new ArgumentNullException(nameof(vlogRepository));
//            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
//            logger = (loggerFactory != null) ? loggerFactory.CreateLogger(nameof(ReactionService)) : throw new ArgumentNullException(nameof(loggerFactory));
//        }

//        /// <summary>
//        /// Deletes a <see cref="Reaction"/> from our data store.
//        /// </summary>
//        /// <remarks>
//        /// Throws a <see cref="NotAllowedException"/> if the <paramref name="userId"/>
//        /// does not match the <see cref="Reaction.UserId"/> in our data store.
//        /// </remarks>
//        /// <param name="userId">Internal <see cref="SwabbrUser"/> id</param>
//        /// <param name="reactionId">Internal <see cref="Reaction"/> id</param>
//        /// <returns><see cref="Task"/></returns>
//        public async Task DeleteReactionAsync(Guid userId, Guid reactionId)
//        {
//            userId.ThrowIfNullOrEmpty();
//            reactionId.ThrowIfNullOrEmpty();

//            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
//            {
//                // TODO Would we want to check user existence here? It is not required, could be useful for quicker debug feedback though?
//                var reaction = await _reactionRepository.GetAsync(reactionId).ConfigureAwait(false);
//                if (reaction.UserId != userId) { throw new NotAllowedException("User does not own reaction"); }

//                // TODO This should also be removed in AMS
//                logger.LogError("Reaction Service should also remove the resources in AMS & storage!");

//                await _reactionRepository.SoftDeleteAsync(reactionId).ConfigureAwait(false);
//                scope.Complete();
//            }
//        }

//        /// <summary>
//        /// Gets a <see cref="Reaction"/> from our data store.
//        /// </summary>
//        /// <param name="reactionId">Internal <see cref="Reaction"/> id</param>
//        /// <returns><see cref="Reaction"/></returns>
//        public async Task<ReactionWithDownload> GetReactionAsync(Guid reactionId)
//        {
//            reactionId.ThrowIfNullOrEmpty();
//            var reaction = await _reactionRepository.GetAsync(reactionId).ConfigureAwait(false);
//            var result = (ReactionWithDownload)reaction;
//            result.VideoAccessUri = await _storageService.GetDownloadAccessUriForReactionVideoAsync(reactionId).ConfigureAwait(false);
//            result.ThumbnailAccessUri = await _storageService.GetDownloadAccessUriForReactionThumbnailAsync(reactionId).ConfigureAwait(false);
//            return result;
//        }

//        /// <summary>
//        /// Gets all <see cref="Reaction"/> entities that belong to a given
//        /// <see cref="Vlog"/>.
//        /// </summary>
//        /// <param name="vlogId">Internal <see cref="Vlog"/> id</param>
//        /// <returns><see cref="Reaction"/> collection</returns>
//        public Task<IEnumerable<Reaction>> GetReactionsForVlogAsync(Guid vlogId)
//        {
//            vlogId.ThrowIfNullOrEmpty();
//            return _reactionRepository.GetForVlogAsync(vlogId);
//        }

//        /// <summary>
//        /// Gets the amount of <see cref="Reaction"/>s for a given <paramref name="vlogId"/>.
//        /// </summary>
//        /// <remarks>
//        /// This throws a <see cref="EntityNotFoundException"/> if the <paramref name="vlogId"/>
//        /// does not exist in our data store.
//        /// </remarks>
//        /// <param name="vlogId">Internal <see cref="Vlog"/> id</param>
//        /// <returns><see cref="Reaction"/> count</returns>
//        public async Task<int> GetReactionCountForVlogAsync(Guid vlogId)
//        {
//            vlogId.ThrowIfNullOrEmpty();
//            if (!await _vlogRepository.ExistsAsync(vlogId).ConfigureAwait(false)) { throw new EntityNotFoundException(nameof(vlogId)); }
//            return await _reactionRepository.GetReactionCountForVlogAsync(vlogId).ConfigureAwait(false);
//        }

//        /// <summary>
//        /// Posts a new <see cref="Reaction"/> to a <see cref="Vlog"/>.
//        /// </summary>
//        /// <param name="userId">Internal <see cref="SwabbrUser"/> id</param>
//        /// <param name="targetVlogId">Internal <see cref="Vlog"/> id</param>
//        /// <param name="isPrivate">If the reaction is private</param>
//        /// <returns>The created and queried <see cref="Reaction"/></returns>
//        public async Task<Reaction> PostReactionAsync(Guid userId, Guid targetVlogId, bool isPrivate)
//        {
//            userId.ThrowIfNullOrEmpty();
//            targetVlogId.ThrowIfNullOrEmpty();

//            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
//            {
//                var vlog = await _vlogRepository.GetAsync(targetVlogId).ConfigureAwait(false); // Throws if vlog is marked as deleted
//                var reaction = await _reactionRepository.CreateAsync(new Reaction
//                {
//                    UserId = userId,
//                    TargetVlogId = targetVlogId,
//                    IsPrivate = isPrivate
//                }).ConfigureAwait(false);
//                scope.Complete();
//                return reaction;
//            }
//        }

//        /// <summary>
//        /// Updates a <see cref="Reaction"/> in our data store.
//        /// </summary>
//        /// <param name="userId">Internal <see cref="SwabbrUser"/> id</param>
//        /// <param name="reactionId">Internal <see cref="Reaction"/> id</param>
//        /// <param name="isPrivate">If the reaction is private</param>
//        /// <returns>The updated and queried <see cref="Reaction"/></returns>
//        public async Task<Reaction> UpdateReactionAsync(Guid userId, Guid reactionId, bool isPrivate)
//        {
//            userId.ThrowIfNullOrEmpty();
//            reactionId.ThrowIfNullOrEmpty();

//            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
//            {
//                var reaction = await _reactionRepository.GetAsync(reactionId).ConfigureAwait(false);
//                if (reaction.UserId != userId) { throw new NotAllowedException("User does not own reaction"); }

//                reaction.IsPrivate = isPrivate;

//                reaction = await _reactionRepository.UpdateAsync(reaction).ConfigureAwait(false);
//                scope.Complete();
//                return reaction;
//            }
//        }

//        /// <summary>
//        /// Gets the <see cref="SwabbrUser"/> owner of a <see cref="Vlog"/> to
//        /// which a given <see cref="Reaction"/> was placed.
//        /// </summary>
//        /// <param name="reactionId">Internal <see cref="Reaction"/> id</param>
//        /// <returns><see cref="SwabbrUser"/></returns>
//        public async Task<SwabbrUser> GetOwnerOfVlogByReactionAsync(Guid reactionId)
//        {
//            reactionId.ThrowIfNullOrEmpty();

//            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
//            {
//                // TODO This could be a single query, but in this way we have clearer error checking.
//                var vlog = await _vlogRepository.GetVlogFromReactionAsync(reactionId).ConfigureAwait(false);
//                var user = await _userRepository.GetUserFromVlogAsync(vlog.Id).ConfigureAwait(false);
//                scope.Complete();
//                return user;
//            }
//        }

//        public Task<int> ExtractVideoLengthInSecondsAsync(Guid reactionId)
//        {
//            throw new NotImplementedException();
//        }

//        public Task<Uri> PostReactionGetSasAsync(Guid userId, Guid targetVlogId, bool isPrivate)
//        {
//            throw new NotImplementedException();
//        }

//        public Task OnFinishedUploadingReactionAsync(Guid reactionId)
//        {
//            throw new NotImplementedException();
//        }
//    }

//}
