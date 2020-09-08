using System;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.Services
{
    /// <summary>
    ///     Contract for managing storage.
    /// </summary>
    public interface IStorageService
    {
        Task CleanupReactionStorageOnSuccessAsync(Guid reactionId);

        Task CleanupReactionStorageOnFailureAsync(Guid reactionId);

        Task CleanupVlogStorageOnDeleteAsync(Guid vlogId);

        Task<Uri> GetDownloadAccessUriForReactionContainerAsync(Guid reactionId);

        Task<Uri> GetDownloadAccessUriForReactionVideoAsync(Guid reactionId);

        Task<Uri> GetDownloadAccessUriForReactionThumbnailAsync(Guid reactionId);

        Task<Uri> GetDownloadAccessUriForVlogThumbnailAsync(Guid vlogId);
    }
}
