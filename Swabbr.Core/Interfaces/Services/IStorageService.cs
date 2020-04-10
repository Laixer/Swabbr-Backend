using System;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.Services
{

    /// <summary>
    /// Contract for managing storage.
    /// </summary>
    public interface IStorageService
    {

        Task CleanupReactionStorageAsync(Guid reactionId);

        Task<Uri> GetDownloadAccessUriForReactionContainerAsync(Guid reactionId);

        Task<Uri> GetDownloadAccessUriForReactionVideoAsync(Guid reactionId);

        Task<Uri> GetDownloadAccessUriForReactionThumbnailAsync(Guid reactionId);


    }

}
