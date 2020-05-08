using Swabbr.Core.Notifications.JsonWrappers;
using Swabbr.Core.Types;
using System;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.Services
{

    /// <summary>
    /// Contract for handling everything that is related to <see cref="Livestream"/>
    /// playback.
    /// </summary>
    public interface ILivestreamPlaybackService
    {

        Task<LivestreamDownstreamDetails> GetLivestreamDownstreamParametersAsync(Guid livestreamId, Guid watchingUserId);

        Task<VlogPlaybackDetails> GetVlogDownstreamParametersAsync(Guid vlogId, Guid watchingUserId);

    }

}
