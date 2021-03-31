using Swabbr.Core.Entities;
using Swabbr.Core.Interfaces.Services;
using Swabbr.Core.Storage;
using System;
using System.Threading.Tasks;

namespace Swabbr.Core.Services
{
    // TODO Move timespan constants to some config.
    /// <summary>
    ///     Generates upload/download uri for entities.
    /// </summary>
    public class EntityStorageUriService : IEntityStorageUriService
    {
        private readonly IBlobStorageService _blobStorageService;

        public EntityStorageUriService(IBlobStorageService blobStorageService)
            => _blobStorageService = blobStorageService ?? throw new ArgumentNullException(nameof(blobStorageService));

        public Task<Uri> GetReactionThumbnailAccessUriAsync(Guid reactionId)
            => _blobStorageService.GetAccessLinkAsync(
                containerName: StorageConstants.ReactionStorageFolderName,
                fileName: StorageHelper.GetThumbnailFileName(reactionId),
                timeSpanValid: TimeSpan.FromHours(2));

        public Task<Uri> GetReactionVideoAccessUriAsync(Guid reactionId)
             => _blobStorageService.GetAccessLinkAsync(
                containerName: StorageConstants.ReactionStorageFolderName,
                fileName: StorageHelper.GetVideoFileName(reactionId),
                timeSpanValid: TimeSpan.FromHours(2));

        public Task<Uri> GetUserProfileImageAccessUriAsync(Guid userId)
            => _blobStorageService.GetAccessLinkAsync(
                containerName: StorageConstants.UserProfileImageStorageFolderName,
                fileName: StorageHelper.GetUserProfileImageFileName(userId),
                timeSpanValid: TimeSpan.FromHours(48));

        public Task<Uri> GetUserProfileImageAccessUriOrNullAsync(User user)
        {
            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return (user.HasProfileImage)
                ? GetUserProfileImageAccessUriAsync(user.Id)
                : Task.FromResult<Uri>(null);
        }

        public Task<Uri> GetUserProfileImageUploadUriAsync(Guid userId)
            => _blobStorageService.GenerateUploadLinkAsync(
                    containerName: StorageConstants.UserProfileImageStorageFolderName,
                    fileName: StorageHelper.GetUserProfileImageFileName(userId),
                    timeSpanValid: TimeSpan.FromHours(2),
                    contentType: "image/jpeg");

        public Task<Uri> GetVlogThumbnailAccessUriAsync(Guid vlogId)
            => _blobStorageService.GetAccessLinkAsync(
                containerName: StorageConstants.VlogStorageFolderName,
                fileName: StorageHelper.GetThumbnailFileName(vlogId),
                timeSpanValid: TimeSpan.FromHours(2));

        public Task<Uri> GetVlogVideoAccessUriAsync(Guid vlogId)
             => _blobStorageService.GetAccessLinkAsync(
                containerName: StorageConstants.VlogStorageFolderName,
                fileName: StorageHelper.GetVideoFileName(vlogId),
                timeSpanValid: TimeSpan.FromHours(2));
    }
}
