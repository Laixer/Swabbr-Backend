using Swabbr.Core.Entities;
using Swabbr.Core.Notifications.JsonWrappers;
using System;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.Services
{

    /// <summary>
    /// Handles all functionality for our <see cref="Livestream"/> objects.
    /// </summary>
    public interface ILivestreamService
    {

        Task<Livestream> TryStartLivestreamForUserAsync(Guid userId);

        Task OnUserStartStreamingAsync(Guid livestreamId, Guid userId);

        Task OnUserStopStreamingAsync(Guid livestreamId, Guid userId);

        Task AbortLivestreamAsync(Guid livestreamId);

        Task<bool> IsLivestreamValidForFollowersAsync(Guid livestreamId, Guid userId);

        Task<ParametersRecordVlog> GetUpstreamParametersAsync(Guid livestreamId, Guid userId);

    }

}
