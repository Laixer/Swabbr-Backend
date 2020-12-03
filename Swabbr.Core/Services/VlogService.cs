using Swabbr.Core.Entities;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Interfaces.Services;
using Swabbr.Core.Storage;
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
        protected readonly AppContext _appContext;
        protected readonly IVlogRepository _vlogRepository;
        protected readonly IVlogLikeRepository _vlogLikeRepository;
        protected readonly IUserRepository _userRepository;
        protected readonly INotificationService _notificationService;
        protected readonly IBlobStorageService _blobStorageService;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public VlogService(AppContext appContext,
            IVlogRepository vlogRepository,
            IVlogLikeRepository vlogLikeRepository,
            IUserRepository userRepository,
            INotificationService notificationService,
            IBlobStorageService blobStorageService)
        {
            _appContext = appContext ?? throw new ArgumentNullException(nameof(appContext));
            _vlogRepository = vlogRepository ?? throw new ArgumentNullException(nameof(vlogRepository));
            _vlogLikeRepository = vlogLikeRepository ?? throw new ArgumentNullException(nameof(vlogLikeRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(vlogRepository));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _blobStorageService = blobStorageService ?? throw new ArgumentNullException(nameof(blobStorageService));
        }

        // FUTURE: Check if the user is allowed to watch the vlog
        /// <summary>
        ///     Adds a view to a vlog.
        /// </summary>
        /// <param name="vlogId">The vlog that is watched.</param>
        public virtual Task AddView(Guid vlogId)
            => _vlogRepository.AddView(vlogId);

        /// <summary>
        ///     Soft deletes a vlog in our data store.
        /// </summary>
        /// <remarks>
        ///     This expects the current user to own the vlog.
        /// </remarks>
        /// <param name="vlogId">The vlog to delete.</param>
        public virtual Task DeleteAsync(Guid vlogId)
            => _vlogRepository.DeleteAsync(vlogId);

        /// <summary>
        ///     Checks if a vlog exists in our data store.
        /// </summary>
        /// <param name="vlogId">The vlog id to check.</param>
        public virtual Task<bool> ExistsAsync(Guid vlogId)
            => _vlogRepository.ExistsAsync(vlogId);

        /// <summary>
        ///     Gets a vlog from our data store.
        /// </summary>
        /// <param name="vlogId">The vlog id.</param>
        /// <returns>The vlog.</returns>
        public virtual Task<Vlog> GetAsync(Guid vlogId)
            => _vlogRepository.GetAsync(vlogId);

        /// <summary>
        ///     Gets all recommended vlogs for a user.
        /// </summary>
        /// <remarks>
        ///     The current user will be extracted from the context.
        /// </remarks>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>Recommended vlogs.</returns>
        public virtual IAsyncEnumerable<Vlog> GetRecommendedForUserAsync(Navigation navigation)
            => _vlogRepository.GetMostRecentVlogsForUserAsync(navigation);

        /// <summary>
        ///     Gets all recommended vlogs for a user including
        ///     their thumbnail details.
        /// </summary>
        /// <remarks>
        ///     The current user will be extracted from the context.
        /// </remarks>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>Vlogs with thumbnail details.</returns>
        public virtual async IAsyncEnumerable<VlogWithThumbnailDetails> GetRecommendedForUserWithThumbnailsAsync(Navigation navigation)
        {
            await foreach (var vlog in GetRecommendedForUserAsync(navigation))
            {
                yield return new VlogWithThumbnailDetails
                {
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
        public virtual IAsyncEnumerable<Vlog> GetVlogsByUserAsync(Guid userId, Navigation navigation)
            => _vlogRepository.GetVlogsByUserAsync(userId, navigation);

        /// <summary>
        ///     Gets all vlogs that belong to a user including
        ///     their thumbnail details.
        /// </summary>
        /// <param name="userId">The corresponding user.</param>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>All vlogs belonging to the user.</returns>
        public virtual async IAsyncEnumerable<VlogWithThumbnailDetails> GetVlogsByUserWithThumbnailsAsync(Guid userId, Navigation navigation)
        {
            await foreach (var vlog in GetVlogsByUserAsync(userId, navigation))
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
        public virtual IAsyncEnumerable<VlogLike> GetVlogLikesForVlogAsync(Guid vlogId, Navigation navigation)
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
        public virtual Task<VlogLikeSummary> GetVlogLikeSummaryForVlogAsync(Guid vlogId)
            => _vlogLikeRepository.GetSummaryForVlogAsync(vlogId);

        /// <summary>
        ///     Gets a vlog including its thumbnail details.
        /// </summary>
        /// <param name="vlogId">The vlog id.</param>
        /// <returns>Vlog with thumbnail details.</returns>
        public virtual async Task<VlogWithThumbnailDetails> GetWithThumbnailAsync(Guid vlogId)
            => new VlogWithThumbnailDetails
            {
                Vlog = await GetAsync(vlogId),
                ThumbnailUri = null // TODO
            };

        /// <summary>
        ///     Used when the current users like a vlog.
        /// </summary>
        /// <param name="vlogId">The vlog to like.</param>
        public virtual async Task LikeAsync(Guid vlogId)
        {
            var vlogLikeId = await _vlogLikeRepository.CreateAsync(new VlogLike
            {
                Id = new VlogLikeId
                {
                    UserId = _appContext.UserId,
                    VlogId = vlogId
                }
            });

            var vlog = await GetAsync(vlogId);
            // FUTURE: Enqueue
            await _notificationService.NotifyVlogLikedAsync(vlog.UserId, vlogLikeId);
        }

        /// <summary>
        ///     Called when a vlog has finished uploading.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         The vlog will be owned by the current user.
        ///     </para>
        ///     <para>
        ///         If the file does not exist in our blob storage this
        ///         throws a <see cref="FileNotFoundException"/>.
        ///     </para>
        /// </remarks>
        /// <param name="vlogId">The uploaded vlog.</param>
        /// <param name="isPrivate">Accessibility of the vlog.</param>
        public virtual async Task PostVlogAsync(Guid vlogId, bool isPrivate = false)
        {
            if (!await _blobStorageService.FileExistsAsync(StorageConstants.VlogStorageFolderName, vlogId.ToString())) 
            {
                throw new FileNotFoundException();
            }

            var vlog = new Vlog
            {
                Id = vlogId,
                IsPrivate = isPrivate,
            };

            // Note: The current user id is assigned in the reaction repository.
            // TODO This comment could not have been made without full knowledge of the repo, which we can't always have!
            await _vlogRepository.CreateAsync(vlog);

            // FUTURE: Enqueue
            await _notificationService.NotifyFollowersVlogPostedAsync(_appContext.UserId, vlogId);
        }

        /// <summary>
        ///     Used when the current user unlikes a vlog.
        /// </summary>
        /// <param name="vlogId">The vlog to unlike.</param>
        public virtual Task UnlikeAsync(Guid vlogId)
            => _vlogLikeRepository.DeleteAsync(new VlogLikeId
            {
                VlogId = vlogId,
                UserId = _appContext.UserId
            });

        // TODO Push functionality to repo?
        /// <summary>
        ///     Updates a vlog in our data store.
        /// </summary>
        /// <remarks>
        ///     This expects the current user to own the vlog.
        /// </remarks>
        /// <param name="vlog">The vlog with updates properties.</param>
        public virtual async Task UpdateAsync(Vlog vlog)
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
