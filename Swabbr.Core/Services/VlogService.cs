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
        /// <param name="navigation">Navigation control.</param>
        /// <returns>Recommended vlogs.</returns>
        public IAsyncEnumerable<Vlog> GetRecommendedForUserAsync(Guid userId, Navigation navigation)
            => _vlogRepository.GetMostRecentVlogsForUserAsync(userId, navigation);

        /// <summary>
        ///     Gets all recommended vlogs for a user including
        ///     their thumbnail details.
        /// </summary>
        /// <param name="userId">The corresponding user.</param>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>Vlogs with thumbnail details.</returns>
        public async IAsyncEnumerable<VlogWithThumbnailDetails> GetRecommendedForUserWithThumbnailsAsync(Guid userId, Navigation navigation)
        {
            await foreach (var vlog in GetRecommendedForUserAsync(userId, navigation))
            {
                yield return new VlogWithThumbnailDetails {
                    Vlog = vlog,
                    ThumbnailUri = null // TODO Implement
                };
            }
        }

        /// <summary>
        ///     Gets all vlogs that belong to a user.
        /// </summary>
        /// <param name="userId">The vlog owner.</param>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>Vlog collection.</returns>
        public IAsyncEnumerable<Vlog> GetVlogsFromUserAsync(Guid userId, Navigation navigation)
            => _vlogRepository.GetVlogsFromUserAsync(userId, navigation);

        /// <summary>
        ///     Gets all vlogs that belong to a user including
        ///     their thumbnail details.
        /// </summary>
        /// <param name="userId">The corresponding user.</param>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>All vlogs belonging to the user.</returns>
        public async IAsyncEnumerable<VlogWithThumbnailDetails> GetVlogsFromUserWithThumbnailsAsync(Guid userId, Navigation navigation)
        {
            await foreach (var vlog in GetVlogsFromUserAsync(userId, navigation))
            {
                yield return new VlogWithThumbnailDetails
                {
                    Vlog = vlog,
                    ThumbnailUri = null // TODO Implement
                };
            }
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
        /// <param name="navigation">Navigation control.</param>
        /// <returns><see cref="VlogLike"/> collection</returns>
        public IAsyncEnumerable<VlogLike> GetVlogLikesForVlogAsync(Guid vlogId, Navigation navigation)
            => _vlogLikeRepository.GetForVlogAsync(vlogId, navigation);

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
            => _vlogLikeRepository.GetSummaryForVlogAsync(vlogId);

        /// <summary>
        ///     Gets a vlog including its thumbnail details.
        /// </summary>
        /// <param name="vlogId">The vlog id.</param>
        /// <returns>Vlog with thumbnail details.</returns>
        public async Task<VlogWithThumbnailDetails> GetWithThumbnailAsync(Guid vlogId)
            => new VlogWithThumbnailDetails
            {
                Vlog = await GetAsync(vlogId),
                ThumbnailUri = null // TODO
            };

        /// <summary>
        ///     Likes a vlog.
        /// </summary>
        /// <param name="vlogId">The vlog to like.</param>
        /// <param name="userId">The user that likes the vlog.</param>
        public async Task LikeAsync(Guid vlogId, Guid userId)
        {
            // It's not allowed to like your own vlog
            var vlog = await _vlogRepository.GetAsync(vlogId);
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
            });

            // TODO Move to some queue
            await _notificationService.NotifyVlogLikedAsync(vlog.UserId, vlogLikeId);
        }

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

            var currentVlog = await _vlogRepository.GetAsync(vlog.Id);

            // Copy all updateable properties.
            // TODO Expand
            currentVlog.IsPrivate = vlog.IsPrivate;

            await _vlogRepository.UpdateAsync(vlog);
        }
    }
}
#pragma warning restore CA1051 // Do not declare visible instance fields
