using Swabbr.Core.Types;
using System;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.Services
{

    /// <summary>
    /// Contract for a service that handles the uploading of reactions.
    /// </summary>
    public interface IReactionUploadService
    {

        Task<StreamWithEntityIdWrapper> GetReactionUploadStreamAsync(Guid userId, Guid targetVlogId);

        Task OnFinishedUploadingReactionAsync(Guid userId, Guid reactionId);

        Task OnFinishedTranscodingReactionAsync(Guid reactionId);

        Task OnFailedTranscodingReactionAsync(Guid reactionId);

    }
}
