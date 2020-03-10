using Swabbr.Core.Entities;
using System;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.Services
{

    /// <summary>
    /// Handles all functionality for our <see cref="Livestream"/> objects.
    /// </summary>
    public interface ILivestreamingService
    {

        Task<Livestream> CreateAndStartLivestreamForUserAsync(Guid userId);

        Task OnUserStartStreamingAsync(Guid livestreamId, Guid userId);

        Task OnUserStopStreamingAsync(Guid livestreamId, Guid userId);

        Task AbortLivestreamAsync(Guid livestreamId);

        Task DiscardLivestreamAsync(Guid livestreamId);

        Task<bool> IsLivestreamValidForFollowersAsync(Guid livestreamId, Guid userId);

        // TODO All the getters

    }

}
