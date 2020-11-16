using Swabbr.Core.Entities;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Extensions;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Interfaces.Services;
using Swabbr.Core.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;

#pragma warning disable CA1051 // Do not declare visible instance fields
namespace Swabbr.Core.Services
{
    /// <summary>
    /// Contains request processing functionality for <see cref="Vlog"/>s and
    /// <see cref="VlogLike"/>s.
    /// </summary>
    public class VlogService : IVlogService
    {
        protected readonly IVlogRepository _vlogRepository;
        protected readonly IVlogLikeRepository _vlogLikeRepository;
        protected readonly IUserRepository _userRepository;
        protected readonly INotificationService _notificationService;
        protected readonly IStorageService _storageService;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public VlogService(IVlogRepository vlogRepository,
            IVlogLikeRepository vlogLikeRepository,
            IUserRepository userRepository,
            INotificationService notificationService,
            IStorageService storageService)
        {
            _vlogRepository = vlogRepository ?? throw new ArgumentNullException(nameof(vlogRepository));
            _vlogLikeRepository = vlogLikeRepository ?? throw new ArgumentNullException(nameof(vlogLikeRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(vlogRepository));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
        }

        /// <summary>
        /// Adds a single view to a <see cref="Vlog"/>.
        /// </summary>
        /// <param name="vlogId">Internal <see cref="Vlog"/> id</param>
        /// <returns><see cref="Task"/></returns>
        public Task AddView(Guid vlogId) => _vlogRepository.AddView(vlogId);

        /// <summary>
        /// Soft deletes a <see cref="Vlog"/> in our data store.
        /// </summary>
        /// <remarks>
        /// Throws a <see cref="UserNotOwnerException"/> if our <paramref name="userId"/>
        /// does not own the <paramref name="vlogId"/>.
        /// </remarks>
        /// <param name="vlogId">Internal <see cref="Vlog"/> id</param>
        /// <param name="userId">Internal <see cref="SwabbrUser"/> id</param>
        /// <returns><see cref="Task"/></returns>
        public async Task DeleteAsync(Guid vlogId, Guid userId)
        {
            vlogId.ThrowIfNullOrEmpty();
            userId.ThrowIfNullOrEmpty();

            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            var vlog = await _vlogRepository.GetAsync(vlogId).ConfigureAwait(false);
            if (vlog.UserId != userId) { throw new UserNotOwnerException(nameof(Vlog)); }

            await _vlogRepository.SoftDeleteAsync(vlogId).ConfigureAwait(false);
            scope.Complete();
        }

        /// <summary>
        /// Gets a <see cref="Vlog"/> from our data store.
        /// </summary>
        /// <param name="vlogId">Internal <see cref="Vlog"/> id</param>
        /// <returns><see cref="Vlog"/></returns>
        public Task<Vlog> GetAsync(Guid vlogId) => _vlogRepository.GetAsync(vlogId);

        /// <summary>
        /// Gets all <see cref="VlogLike"/> entities that belong to a given
        /// <see cref="Vlog"/>.
        /// </summary>
        /// <param name="vlogId">Internal <see cref="Vlog"/> id</param>
        /// <returns><see cref="VlogLike"/> collection</returns>
        public Task<IEnumerable<VlogLike>> GetAllVlogLikesForVlogAsync(Guid vlogId) => _vlogLikeRepository.GetAllForVlogAsync(vlogId);

        /// <summary>
        /// Creates a <see cref="VlogLike"/> between a <see cref="Vlog"/>
        /// and a <see cref="SwabbrUser"/>.
        /// </summary>
        /// <remarks>
        /// Throws an <see cref="OperationAlreadyExecutedException"/> if the user
        /// already liked the vlog.
        /// 
        /// Throws an <see cref="NotAllowedException"/> if the user owns the vlog.
        /// </remarks>
        /// <param name="vlogId">Internal <see cref="Vlog"/> id</param>
        /// <param name="userId">Internal <see cref="SwabbrUser"/> id</param>
        /// <returns><see cref="Task"/></returns>
        public async Task LikeAsync(Guid vlogId, Guid userId)
        {
            vlogId.ThrowIfNullOrEmpty();
            userId.ThrowIfNullOrEmpty();

            var vlogLikeId = new VlogLikeId { VlogId = vlogId, UserId = userId };
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            if (await _vlogLikeRepository.ExistsAsync(vlogLikeId).ConfigureAwait(false)) { throw new OperationAlreadyExecutedException("User already liked given vlog"); }
            if ((await _vlogRepository.GetAsync(vlogId).ConfigureAwait(false)).UserId == userId) { throw new NotAllowedException("Can't like your own vlog"); }

            await _vlogLikeRepository.CreateAsync(new VlogLike
            {
                Id = vlogLikeId
            }).ConfigureAwait(false);

            scope.Complete();

            // We need the vlog id for our notification service.
            var vlog = await GetAsync(vlogId).ConfigureAwait(false);

            await _notificationService.NotifyVlogLikedAsync(vlog.UserId, vlogLikeId).ConfigureAwait(false);
        }

        /// <summary>
        /// Lists all <see cref="Vlog"/> entities that are owned by a given
        /// <see cref="SwabbrUser"/>.
        /// </summary>
        /// <param name="userId">Internal <see cref="SwabbrUser"/> id</param>
        /// <returns><see cref="Vlog"/> collection</returns>
        public Task<IEnumerable<Vlog>> GetVlogsFromUserAsync(Guid userId) => _vlogRepository.GetVlogsFromUserAsync(userId);

        /// <summary>
        /// Unlikes a <see cref="Vlog"/> by deleting the corresponding 
        /// <see cref="VlogLike"/>.
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <param name="vlogId">Internal <see cref="Vlog"/> id</param>
        /// <param name="userId">Internal <see cref="SwabbrUser"/> id</param>
        /// <returns><see cref="Task"/></returns>
        public async Task UnlikeAsync(Guid vlogId, Guid userId)
        {
            vlogId.ThrowIfNullOrEmpty();
            userId.ThrowIfNullOrEmpty();

            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            var vlogLikeId = new VlogLikeId
            {
                VlogId = vlogId,
                UserId = userId
            };
            if (!await _vlogLikeRepository.ExistsAsync(vlogLikeId).ConfigureAwait(false)) { throw new EntityNotFoundException(); }

            await _vlogLikeRepository.DeleteAsync(vlogLikeId).ConfigureAwait(false);

            scope.Complete();
        }

        /// <summary>
        /// Updates a <see cref="Vlog"/> in our data store.
        /// TODO Is this optimal?
        /// </summary>
        /// <param name="vlogId">Internal <see cref="Vlog"/> id</param>
        /// <param name="userId">Internal <see cref="SwabbrUser"/> id</param>
        /// <param name="isPrivate">Whether or not the <see cref="Vlog"/> should become
        /// private</param>
        /// <returns>Updated and queried <see cref="Vlog"/></returns>
        public async Task<Vlog> UpdateAsync(Guid vlogId, Guid userId, bool isPrivate)
        {
            vlogId.ThrowIfNullOrEmpty();
            userId.ThrowIfNullOrEmpty();

            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            var user = await _userRepository.GetAsync(userId).ConfigureAwait(false);
            var vlog = await _vlogRepository.GetAsync(vlogId).ConfigureAwait(false);
            if (vlog.UserId != user.Id) { throw new NotAllowedException("User doesn't own vlog"); }

            vlog.IsPrivate = isPrivate;
            // TODO Implement shared users here

            var updatedVlog = await _vlogRepository.UpdateAsync(vlog).ConfigureAwait(false);

            // Commit and return
            scope.Complete();
            return updatedVlog;
        }

        /// <summary>
        /// Checks if a <see cref="Vlog"/> exists or not.
        /// </summary>
        /// <param name="vlogId">Internal <see cref="Vlog"/> id</param>
        /// <returns>Boolean result</returns>
        public Task<bool> ExistsAsync(Guid vlogId) => _vlogRepository.ExistsAsync(vlogId);

        /// <summary>
        /// Gets a collection of recommended <see cref="Vlog"/>s for a given
        /// <paramref name="userId"/>.
        /// </summary>
        /// <param name="userId">Internal <see cref="SwabbrUser"/> id</param>
        /// <param name="maxCount">Maximum result count</param>
        /// <returns><see cref="Vlog"/> collection</returns>
        public Task<IEnumerable<Vlog>> GetRecommendedForUserAsync(Guid userId, uint maxCount) => _vlogRepository.GetMostRecentVlogsForUserAsync(userId, maxCount);

        /// <summary>
        /// Gets a <see cref="Vlog"/> that belongs to a <see cref="Livestream"/>.
        /// </summary>
        /// <remarks>
        /// This returns <see cref="EntityNotFoundException"/> if no <see cref="Vlog"/>
        /// is currently bound to the specified <see cref="Livestream"/>.
        /// </remarks>
        /// <param name="livestreamId">Internal <see cref="Livestream"/> id</param>
        /// <returns><see cref="Vlog"/></returns>
        public Task<Vlog> GetVlogFromLivestreamAsync(Guid livestreamId)
        {
            livestreamId.ThrowIfNullOrEmpty();
            return _vlogRepository.GetVlogFromLivestreamAsync(livestreamId);
        }

        /// <summary>
        ///     Gets a <see cref="VlogLikeSummary"/> for a given vlog.
        /// </summary>
        /// <remarks>
        ///     The <see cref="VlogLikeSummary.Users"/> does not need
        ///     to contain all the <see cref="SwabbrUserSimplified"/> users.
        /// </remarks>
        /// <param name="vlogId">Internal <see cref="Vlog"/> id</param>
        /// <returns><see cref="VlogLikeSummary"/></returns>
        public Task<VlogLikeSummary> GetVlogLikeSummaryForVlogAsync(Guid vlogId)
            => _vlogLikeRepository.GetVlogLikeSummaryForVlogAsync(vlogId);
    }
}
#pragma warning restore CA1051 // Do not declare visible instance fields
