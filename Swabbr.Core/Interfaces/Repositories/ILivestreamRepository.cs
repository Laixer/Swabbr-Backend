using Swabbr.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.Repositories
{

    /// <summary>
    /// Repository for <see cref="Livestream"/> entities.
    /// </summary>
    public interface ILivestreamRepository : IRepository<Livestream, Guid>
    {

        Task<Livestream> CreateAsync(Livestream entity);

        Task DeleteAsync(Guid id);

        Task<IEnumerable<Livestream>> GetAvailableLivestreamsAsync();

        Task<Livestream> GetByExternalIdAsync(string externalId);

        Task<string> GetExternalIdAsync(Guid id);

        Task<Livestream> GetLivestreamFromTriggerMinute(Guid userId, DateTimeOffset triggerMinute);

        Task MarkClosedAsync(Guid livestreamId);

        Task MarkCreatedAsync(Guid id, string externalId, string broadcastLocation);

        Task MarkLiveAsync(Guid id);

        Task MarkPendingClosureAsync(Guid livestreamId);

        Task MarkPendingUserAsync(Guid livestreamId, Guid userId, DateTimeOffset triggerMinute);

        Task MarkPendingUserConnectAsync(Guid livestreamId);

        Task MarkUserNoResponseTimeoutAsync(Guid livestreamId);

    }

}
