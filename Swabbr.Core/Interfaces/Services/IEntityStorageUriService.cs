using Swabbr.Core.Entities;
using System;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.Services
{
    // TODO Add upload wrappers here too?
    /// <summary>
    ///     Contract for generating upload/access links for entities.
    /// </summary>
    public interface IEntityStorageUriService
    {
        public Task<Uri> GetVlogVideoAccessUriAsync(Guid vlogId);

        public Task<Uri> GetVlogThumbnailAccessUriAsync(Guid vlogId);

        public Task<Uri> GetReactionVideoAccessUriAsync(Guid reactionId);

        public Task<Uri> GetReactionThumbnailAccessUriAsync(Guid reactionId);

        public Task<Uri> GetUserProfileImageAccessUriAsync(Guid userId);

        public Task<Uri> GetUserProfileImageAccessUriOrNullAsync(User user);

        public Task<Uri> GetUserProfileImageUploadUriAsync(Guid userId);
    }
}
