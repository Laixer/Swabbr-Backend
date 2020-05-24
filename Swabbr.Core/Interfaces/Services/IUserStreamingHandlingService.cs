using Swabbr.Core.Types;
using System;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.Services
{

    /// <summary>
    /// Contract for managing user stream requests.
    /// </summary>
    public interface IUserStreamingHandlingService
    {

        Task OnUserConnectedToLivestreamAsync(Guid userId, Guid livestreamId);

        Task OnUserDisconnectedFromLivestreamAsync(Guid userId, Guid livestreamId);

        Task OnUserNeverConnectedCheckAsync(Guid userId, Guid livestreamId);

        Task<LivestreamUpstreamDetails> OnUserStartStreaming(Guid userId, Guid livestreamId);

        Task OnUserStopStreaming(Guid userId, Guid livestreamId);

        Task OnUserVlogTimeExpiredAsync(Guid userId, Guid livestreamId);

    }

}
