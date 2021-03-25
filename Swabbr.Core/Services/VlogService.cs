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
    ///     Service that handles all vlog related operations.
    /// </summary>
    public class VlogService : AppServiceBase, IVlogService
    {
        private readonly IVlogRepository _vlogRepository;
        private readonly INotificationService _notificationService;
        private readonly IBlobStorageService _blobStorageService;

        /// <summary>
        ///     Create new instance.
        /// </summary>
        public VlogService(AppContext appContext,
            IVlogRepository vlogRepository,
            INotificationService notificationService,
            IBlobStorageService blobStorageService)
        {
            AppContext = appContext ?? throw new ArgumentNullException(nameof(appContext));
            _vlogRepository = vlogRepository ?? throw new ArgumentNullException(nameof(vlogRepository));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _blobStorageService = blobStorageService ?? throw new ArgumentNullException(nameof(blobStorageService));
        }

        // FUTURE: Check if the user is allowed to watch the vlog
        /// <summary>
        ///     Adds views for given vlogs.
        /// </summary>
        /// <param name="context">Context for adding vlog views.</param>
        public Task AddViews(AddVlogViewsContext context)
            => _vlogRepository.AddViews(context);

        /// <summary>
        ///     Soft deletes a vlog in our data store.
        /// </summary>
        /// <remarks>
        ///     This expects the current user to own the vlog.
        /// </remarks>
        /// <param name="vlogId">The vlog to delete.</param>
        public Task DeleteAsync(Guid vlogId)
            => _vlogRepository.DeleteAsync(vlogId);

        /// <summary>
        ///     Checks if a vlog exists in our data store.
        /// </summary>
        /// <param name="vlogId">The vlog id to check.</param>
        public Task<bool> ExistsAsync(Guid vlogId)
            => _vlogRepository.ExistsAsync(vlogId);

        // TODO Hardcoded content types.
        /// <summary>
        ///     Generates an upload uri for a new vlog.
        /// </summary>
        /// <param name="vlogId">The new vlog id.</param>
        /// <returns>Upload uri.</returns>
        public async Task<UploadWrapper> GenerateUploadUri(Guid vlogId)
            => new UploadWrapper
            {
                Id = vlogId,
                ThumbnailUploadUri = await _blobStorageService.GenerateUploadLinkAsync(
                    containerName: StorageConstants.VlogStorageFolderName,
                    fileName: StorageHelper.GetThumbnailFileName(vlogId),
                    timeSpanValid: TimeSpan.FromHours(2),
                    contentType: "image/jpeg"),
                VideoUploadUri = await _blobStorageService.GenerateUploadLinkAsync(
                    containerName: StorageConstants.VlogStorageFolderName,
                    fileName: StorageHelper.GetVideoFileName(vlogId),
                    timeSpanValid: TimeSpan.FromHours(2),
                    contentType: "video/mp4")
            };

        /// <summary>
        ///     Gets a vlog from our data store.
        /// </summary>
        /// <param name="id">The vlog id.</param>
        /// <returns>The vlog.</returns>
        public async Task<Vlog> GetAsync(Guid id)
        {
            var vlog = await _vlogRepository.GetAsync(id);

            vlog.ThumbnailUri = await GetThumbnailUriAsync(vlog);
            vlog.VideoUri = await GetVideoUriAsync(vlog);

            return vlog;
        }

        /// <summary>
        ///     Gets a vlog wrapper from our data store.
        /// </summary>
        /// <param name="id">The vlog id.</param>
        /// <returns>The vlog.</returns>
        public async Task<VlogWrapper> GetWrapperAsync(Guid id)
        {
            var vlogWrapper = await _vlogRepository.GetWrapperAsync(id);

            vlogWrapper.Vlog.ThumbnailUri = await GetThumbnailUriAsync(vlogWrapper.Vlog);
            vlogWrapper.Vlog.VideoUri = await GetVideoUriAsync(vlogWrapper.Vlog);

            return vlogWrapper;
        }

        /// <summary>
        ///     Gets all recommended vlogs for a user.
        /// </summary>
        /// <remarks>
        ///     The current user will be extracted from the context.
        /// </remarks>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>Recommended vlogs.</returns>
        public async IAsyncEnumerable<Vlog> GetRecommendedForUserAsync(Navigation navigation)
        {
            await foreach (var vlog in _vlogRepository.GetMostRecentVlogsForUserAsync(navigation))
            {
                vlog.ThumbnailUri = await GetThumbnailUriAsync(vlog);
                vlog.VideoUri = await GetVideoUriAsync(vlog);

                yield return vlog;
            }
        }

        /// <summary>
        ///     Gets recommended vlog wrappers for the current user.
        /// </summary>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>Recommended vlogs.</returns>
        public async IAsyncEnumerable<VlogWrapper> GetRecommendedWrappersForUserAsync(Navigation navigation)
        {
            await foreach (var vlogWrapper in _vlogRepository.GetMostRecentVlogWrappersForUserAsync(navigation))
            {
                vlogWrapper.Vlog.ThumbnailUri = await GetThumbnailUriAsync(vlogWrapper.Vlog);
                vlogWrapper.Vlog.VideoUri = await GetVideoUriAsync(vlogWrapper.Vlog);

                yield return vlogWrapper;
            }
        }

        /// <summary>
        ///     Gets all vlogs that belong to a user.
        /// </summary>
        /// <param name="userId">The vlog owner.</param>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>Vlog collection.</returns>
        public async IAsyncEnumerable<Vlog> GetVlogsByUserAsync(Guid userId, Navigation navigation)
        {
            await foreach (var vlog in _vlogRepository.GetVlogsByUserAsync(userId, navigation))
            {
                vlog.ThumbnailUri = await GetThumbnailUriAsync(vlog);
                vlog.VideoUri = await GetVideoUriAsync(vlog);

                yield return vlog;
            }
        }

        /// <summary>
        ///     Returns a collection of vlog wrappers that are 
        ///     owned by the specified user.
        /// </summary>
        /// <param name="userId">Owner user id.</param>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>Vlogs that belong to the user.</returns>
        public async IAsyncEnumerable<VlogWrapper> GetVlogWrappersByUserAsync(Guid userId, Navigation navigation)
        {
            await foreach (var vlogWrapper in _vlogRepository.GetVlogWrappersByUserAsync(userId, navigation))
            {
                vlogWrapper.Vlog.ThumbnailUri = await GetThumbnailUriAsync(vlogWrapper.Vlog);
                vlogWrapper.Vlog.VideoUri = await GetVideoUriAsync(vlogWrapper.Vlog);

                yield return vlogWrapper;
            }
        }

        /// <summary>
        ///     Called when a vlog has been uploaded. This will
        ///     publish the vlog and notify all followers.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         The vlog will be owned by the current user.
        ///     </para>
        ///     <para>
        ///         If the video file or thumbnail file does not 
        ///         exist in our blob storage this throws a 
        ///         <see cref="FileNotFoundException"/>.
        ///     </para>
        /// </remarks>
        /// <param name="context">Context for posting the vlog.</param>
        public async Task PostVlogAsync(PostVlogContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (!await _blobStorageService.FileExistsAsync(StorageConstants.VlogStorageFolderName, StorageHelper.GetVideoFileName(context.VlogId)))
            {
                throw new FileNotFoundException();
            }
            if (!await _blobStorageService.FileExistsAsync(StorageConstants.VlogStorageFolderName, StorageHelper.GetThumbnailFileName(context.VlogId)))
            {
                throw new FileNotFoundException();
            }

            var vlog = new Vlog
            {
                Id = context.VlogId,
                IsPrivate = context.IsPrivate,
                UserId = context.UserId,
            };

            await _vlogRepository.CreateAsync(vlog);

            // This function will dispatch a notification for each follower.
            await _notificationService.NotifyFollowersVlogPostedAsync(AppContext.UserId, context.VlogId);
        }

        /// <summary>
        ///     Updates a vlog in our data store.
        /// </summary>
        /// <remarks>
        ///     This expects the current user to own the vlog.
        /// </remarks>
        /// <param name="vlog">The vlog with updates properties.</param>
        public Task UpdateAsync(Vlog vlog)
            => _vlogRepository.UpdateAsync(vlog);

        /// <summary>
        ///     Extract the thumbnail uri for a vlog.
        /// </summary>
        /// <param name="vlog">The vlog.</param>
        /// <returns>Thumbnail uri.</returns>
        protected Task<Uri> GetThumbnailUriAsync(Vlog vlog)
            => _blobStorageService.GetAccessLinkAsync(
                containerName: StorageConstants.VlogStorageFolderName,
                fileName: StorageHelper.GetThumbnailFileName(vlog.Id),
                timeSpanValid: TimeSpan.FromHours(2));

        /// <summary>
        ///     Extract the video uri for a vlog.
        /// </summary>
        /// <param name="vlog">The vlog.</param>
        /// <returns>Video uri.</returns>
        protected Task<Uri> GetVideoUriAsync(Vlog vlog)
            => _blobStorageService.GetAccessLinkAsync(
                containerName: StorageConstants.VlogStorageFolderName,
                fileName: StorageHelper.GetVideoFileName(vlog.Id),
                timeSpanValid: TimeSpan.FromHours(2));
    }
}
