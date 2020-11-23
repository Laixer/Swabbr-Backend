using Swabbr.Core.Entities;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Interfaces.Services;
using Swabbr.Core.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

#pragma warning disable CA1051 // Do not declare visible instance fields
namespace Swabbr.Core.Services
{
    /// <summary>
    ///     Service that handles all vlog related operations.
    /// </summary>
    /// <remarks>
    ///     TODO This depends on <see cref="INotificationService"/> 
    ///     which seems incorrect, as this has nothing to do with
    ///     vlogs themself. Use queue in future.
    /// </remarks>
    public class VlogService : IVlogService
    {
        protected readonly IVlogRepository _vlogRepository;
        protected readonly IVlogLikeRepository _vlogLikeRepository;
        protected readonly IUserRepository _userRepository;
        protected readonly INotificationService _notificationService;
        protected readonly IBlobStorageService _blobStorageService;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public VlogService(IVlogRepository vlogRepository,
            IVlogLikeRepository vlogLikeRepository,
            IUserRepository userRepository,
            INotificationService notificationService,
            IBlobStorageService blobStorageService)
        {
            _vlogRepository = vlogRepository ?? throw new ArgumentNullException(nameof(vlogRepository));
            _vlogLikeRepository = vlogLikeRepository ?? throw new ArgumentNullException(nameof(vlogLikeRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(vlogRepository));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _blobStorageService = blobStorageService ?? throw new ArgumentNullException(nameof(blobStorageService));
        }

        /// <summary>
        ///     Adds a view to a vlog.
        /// </summary>
        /// <param name="vlogId">The vlog that is watched.</param>
        public Task AddView(Guid vlogId)
            => _vlogRepository.AddView(vlogId);

        /// <summary>
        ///     Cleans up a vlog from our system, removing it
        ///     from the data store and performing all required
        ///     cleanup operations for it.
        /// </summary>
        /// <remarks>
        ///     Call this after a vlog request timed out.
        /// </remarks>
        /// <param name="vlogId">The vlog to be cleaned up.</param>
        public Task CleanupExpiredVlogAsync(Guid vlogId) => throw new NotImplementedException();

        /// <summary>
        ///     Creates a new vlog in our data store with its status
        ///     set to created. The vlog will be assigned to the user.
        /// </summary>
        /// <remarks>
        ///     Use this before sending a vlog request to create the
        ///     vlog for which the user should upload the content.
        /// </remarks>
        /// <param name="userId">The future owner of the vlog.</param>
        /// <returns>The created vlog.</returns>
        public Task<Vlog> CreateEmptyVlogForUserAsync(Guid userId) => throw new NotImplementedException();

        /// <summary>
        ///     Soft deletes a vlog in our data store.
        /// </summary>
        /// <param name="vlogId">The vlog to delete.</param>
        public Task DeleteAsync(Guid vlogId)
            => _vlogRepository.DeleteAsync(vlogId);

        /// <summary>
        ///     Checks if a vlog exists in our data store.
        /// </summary>
        /// <param name="vlogId">The vlog id to check.</param>
        public Task<bool> ExistsAsync(Guid vlogId)
            => _vlogRepository.ExistsAsync(vlogId);

        /// <summary>
        ///     Gets a vlog from our data store.
        /// </summary>
        /// <param name="vlogId">The vlog id.</param>
        /// <returns>The vlog.</returns>
        public Task<Vlog> GetAsync(Guid vlogId)
            => _vlogRepository.GetAsync(vlogId);

        /// <summary>
        ///     Gets all recommended vlogs for a user.
        /// </summary>
        /// <param name="userId">The corresponding user.</param>
        /// <param name="maxCount">Maximum result set size.</param>
        /// <returns>Recommended vlogs.</returns>
        public Task<IEnumerable<Vlog>> GetRecommendedForUserAsync(Guid userId, uint maxCount)
            => _vlogRepository.GetMostRecentVlogsForUserAsync(userId, maxCount);

        /// <summary>
        ///     Gets all recommended vlogs for a user including
        ///     their thumbnail details.
        /// </summary>
        /// <param name="userId">The corresponding user.</param>
        /// <param name="maxCount">Maximum result set count.</param>
        /// <returns>Vlogs with thumbnail details.</returns>
        public Task<IEnumerable<VlogWithThumbnailDetails>> GetRecommendedForUserWithThumbnailsAsync(Guid userId, uint maxCount) => throw new NotImplementedException();

        /// <summary>
        ///     Gets an upload uri for a vlog.
        /// </summary>
        /// <param name="vlogId">The vlog id.</param>
        /// <returns>Upload uri.</returns>
        public Uri GetUploadUri(Guid vlogId) => throw new NotImplementedException();

        /// <summary>
        ///     Gets all vlogs that belong to a user.
        /// </summary>
        /// <param name="userId">The vlog owner.</param>
        /// <returns>Vlog collection.</returns>
        public Task<IEnumerable<Vlog>> GetVlogsFromUserAsync(Guid userId)
            => _vlogRepository.GetVlogsFromUserAsync(userId);

        /// <summary>
        ///     Gets all vlogs that belong to a user including
        ///     their thumbnail details.
        /// </summary>
        /// <param name="userId">The corresponding user.</param>
        /// <returns>All vlogs belonging to the user.</returns>
        public async Task<IEnumerable<VlogWithThumbnailDetails>> GetVlogsFromUserWithThumbnailsAsync(Guid userId)
        {
            // TODO Async enumerable
            var vlogs = await GetVlogsFromUserAsync(userId).ConfigureAwait(false);

            var result = new List<VlogWithThumbnailDetails>();
            foreach (var vlog in vlogs)
            {
                result.Add(new VlogWithThumbnailDetails
                {
                    Vlog = vlog,
                    // ThumbnailUri = GetThumbnailUri
                });
            }

            throw new NotImplementedException();
            //return result;
        }

        /// <summary>
        ///     Gets all the <see cref="VlogLike"/>s for a <see cref="Vlog"/>.
        /// </summary>
        /// <remarks>
        ///     This does not scale. If that is required, use an implementation
        ///     of <see cref="GetVlogLikeSummaryForVlogAsync(Guid)"/> which does
        ///     not return all <see cref="VlogLike"/> but only a subset.
        /// </remarks>
        /// <param name="vlogId">Internal <see cref="Vlog"/> id</param>
        /// <returns><see cref="VlogLike"/> collection</returns>
        public Task<IEnumerable<VlogLike>> GetVlogLikesForVlogAsync(Guid vlogId)
            => _vlogLikeRepository.GetAllForVlogAsync(vlogId);

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

        /// <summary>
        ///     Gets a vlog including its thumbnail details.
        /// </summary>
        /// <param name="vlogId">The vlog id.</param>
        /// <returns>Vlog with thumbnail details.</returns>
        public Task<VlogWithThumbnailDetails> GetWithThumbnailAsync(Guid vlogId) => throw new NotImplementedException();

        /// <summary>
        ///     Likes a vlog.
        /// </summary>
        /// <param name="vlogId">The vlog to like.</param>
        /// <param name="userId">The user that likes the vlog.</param>
        public async Task LikeAsync(Guid vlogId, Guid userId)
        {
            // It's not allowed to like your own vlog
            var vlog = await _vlogRepository.GetAsync(vlogId).ConfigureAwait(false);
            if (vlog.UserId == userId)
            {
                throw new NotAllowedException("Can't like your own vlog");
            }

            // Explicitly define id because we need it for the notification service
            var vlogLikeId = new VlogLikeId
            {
                VlogId = vlogId,
                UserId = userId
            };

            await _vlogLikeRepository.CreateAsync(new VlogLike
            {
                Id = vlogLikeId
            }).ConfigureAwait(false);

            // TODO Move to some queue
            await _notificationService.NotifyVlogLikedAsync(vlog.UserId, vlogLikeId).ConfigureAwait(false);
        }

        /// <summary>
        ///     Called when a vlog transcoding process failed.
        /// </summary>
        /// <param name="vlogId">The failed transcoded vlog.</param>
        public Task OnTranscodingFailedAsync(Guid vlogId) => throw new NotImplementedException();

        /// <summary>
        ///     Called when a vlog transcoding process succeeded.
        /// </summary>
        /// <param name="vlogId">The transcoded vlog.</param>
        public Task OnTranscodingSucceededAsync(Guid vlogId) => throw new NotImplementedException();

        /// <summary>
        ///     Called when a vlog has finished uploading.
        /// </summary>
        /// <param name="vlogId">The uploaded vlog.</param>
        public Task PostVlogAsync(Guid vlogId) => throw new NotImplementedException();

        /// <summary>
        ///     Unlikes a vlog.
        /// </summary>
        /// <param name="vlogId">The vlog to unlike.</param>
        /// <param name="userId">The user that unlikes the vlog.</param>
        public Task UnlikeAsync(Guid vlogId, Guid userId)
            => _vlogLikeRepository.DeleteAsync(new VlogLikeId
            {
                VlogId = vlogId,
                UserId = userId
            });

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
    }
}
#pragma warning restore CA1051 // Do not declare visible instance fields
