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
    ///     Service that handles all vlog related operations.
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

            await _vlogRepository.DeleteAsync(vlogId).ConfigureAwait(false);
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

        // TODO Push functionality to repo?
        /// <summary>
        ///     Updates a vlog in our data store.
        /// </summary>
        /// <param name="vlog">The vlog with updates properties.</param>
        public async Task UpdateAsync(Vlog vlog)
        {
            if (vlog is null)
            {
                throw new ArgumentNullException(nameof(vlog));
            }

            var currentVlog = await _vlogRepository.GetAsync(vlog.Id).ConfigureAwait(false);

            // Copy all updateable properties.
            // TODO Expand
            currentVlog.IsPrivate = vlog.IsPrivate;

            await _vlogRepository.UpdateAsync(vlog).ConfigureAwait(false);
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
        ///     Gets a <see cref="VlogLikeSummary"/> for a given vlog.
        /// </summary>
        /// <remarks>
        ///     The <see cref="VlogLikeSummary.Users"/> does not need
        ///     to contain all the <see cref="SwabbrUser"/> users.
        /// </remarks>
        /// <param name="vlogId">Internal <see cref="Vlog"/> id</param>
        /// <returns><see cref="VlogLikeSummary"/></returns>
        public Task<VlogLikeSummary> GetVlogLikeSummaryForVlogAsync(Guid vlogId)
            => _vlogLikeRepository.GetVlogLikeSummaryForVlogAsync(vlogId);

        public Task CleanupExpiredVlogAsync(Guid vlogId) => throw new NotImplementedException();
        public Task<Vlog> CreateEmptyVlogForUserAsync(Guid userId) => throw new NotImplementedException();
        public Task DeleteAsync(Guid vlogId) => throw new NotImplementedException();
        public Uri GetUploadUriForVlog(Guid vlogId) => throw new NotImplementedException();
        public Task OnTranscodingFailedAsync(Guid vlogId) => throw new NotImplementedException();
        public Task OnTranscodingSucceededAsync(Guid vlogId) => throw new NotImplementedException();
        public Task OnVlogUploadedAsync(Guid vlogId) => throw new NotImplementedException();
        public Task<VlogWithThumbnailDetails> GetWithThumbnailAsync(Guid vlogId) => throw new NotImplementedException();
        public Task<IEnumerable<VlogWithThumbnailDetails>> GetRecommendedForWithThumbnailUserAsync(Guid userId, uint maxCount) => throw new NotImplementedException();
        public Task<IEnumerable<VlogWithThumbnailDetails>> GetVlogsFromUserWithThumbnailsAsync(Guid userId) => throw new NotImplementedException();
        public Uri GetUploadUri(Guid vlogId) => throw new NotImplementedException();
    }
}
#pragma warning restore CA1051 // Do not declare visible instance fields
