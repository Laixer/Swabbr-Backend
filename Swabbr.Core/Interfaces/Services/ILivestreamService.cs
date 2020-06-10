using Swabbr.Core.Entities;
using Swabbr.Core.Notifications.JsonWrappers;
using Swabbr.Core.Types;
using System;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.Services
{
    /// <summary>
    /// Handles all functionality for our <see cref="Livestream"/> objects.
    /// </summary>
    public interface ILivestreamService
    {

        Task<bool> IsServiceOnlineAsync();

        Task<bool> ExistsLivestreamForTriggerMinute(Guid userId, DateTimeOffset triggerMinute);

        Task<Livestream> GetLivestreamFromExternalIdAsync(string externalId);

        Task<Livestream> GetLivestreamFromTriggerMinute(Guid userId, DateTimeOffset triggerMinute);

        Task<Livestream> ClaimLivestreamForUserAsync(Guid userId, DateTimeOffset triggerMinute);

        Task OnUserNeverConnectedToLivestreamAsync(Guid livestreamId, Guid userId);

        Task OnUserStartStreamingAsync(Guid livestreamId, Guid userId);

        Task OnUserStopStreamingAsync(Guid livestreamId, Guid userId);

        Task OnUserVlogTimeExpiredAsync(Guid livestreamId, Guid userId);

        Task ProcessTimeoutAsync(Guid userId, Guid livestreamId);

        Task<ParametersRecordVlog> GetParametersRecordVlogAsync(Guid livestreamId, DateTimeOffset triggerMinute);

        Task<LivestreamUpstreamDetails> GetUpstreamDetailsAsync(Guid livestreamId, Guid userId);

        Task<bool> IsUserInLivestreamCycleAsync(Guid userId);

        Task OnUserConnectedToLivestreamAsync(Guid livestreamId, Guid userId);

        Task OnUserDisconnectedFromLivestreamAsync(Guid livestreamId, Guid userId);

    }
}
