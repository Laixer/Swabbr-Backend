using Swabbr.Core.Notifications.JsonWrappers;
using Swabbr.Core.Types;
using System;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.Services
{

    /// <summary>
    /// Contract for handling everything that is related to video playback.
    /// </summary>
    public interface IPlaybackService
    {

        Task<LivestreamDownstreamDetails> GetLivestreamDownstreamParametersAsync(Guid livestreamId, Guid watchingUserId);

        Task<VlogPlaybackDetails> GetVlogDownstreamParametersAsync(Guid vlogId, Guid watchingUserId);

        Task<ReactionPlaybackDetails> GetReactionDownstreamParametersAsync(Guid reactionId, Guid watchingUserId);

    }

}
