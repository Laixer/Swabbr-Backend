using Swabbr.Core.Notifications.JsonWrappers;
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

        Task<string> GetTokenAsync(Guid livestreamId, Guid watchingUserId);

        /// <summary>
        /// Gets the playback url for a livestream.
        /// </summary>
        /// <remarks>
        /// This does not require the watching <see cref="Entities.SwabbrUser"/>
        /// internal id, since we might not always want to require this while 
        /// getting a stream playback. When we want to playback a token auth stream,
        /// we will always need to call <see cref="GetTokenAsync(Guid, Guid)"/>
        /// anwaysy, so there is no security issue.
        /// </remarks>
        /// <param name="livestreamId">Internal <see cref="Entities.Livestream"/> id</param>
        /// <returns>External <see cref="Entities.Livestream"/> playback url</returns>
        Task<Uri> GetPlaybackUrlAsync(Guid livestreamId);

        Task<ParametersFollowedProfileLive> GetDownstreamParametersAsync(Guid livestreamId, Guid watchingUserId);

    }

}
