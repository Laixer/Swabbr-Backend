using Swabbr.Core.Types;
using System;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.Services
{

    /// <summary>
    /// Contract for a video transcoder.
    /// </summary>
    public interface ITranscodingService
    {

        Task<StreamWithEntityIdWrapper> GetStreamForReactionUploadAsync(Guid reactionId);

        Task ProcessReactionAsync(Guid reactionId);

        Task<int> ExtractVideoLengthInSecondsAsync(Guid reactionId);

    }

}
