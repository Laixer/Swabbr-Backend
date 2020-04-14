using Swabbr.Core.Entities;
using Swabbr.Core.Enums;
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

        Task<string> GetExternalIdAsync(Guid id);

        Task UpdateLivestreamStatusAsync(Guid id, LivestreamStatus status);

        Task MarkCreatedAsync(Guid id, string externalId, string broadcastLocation);

        Task MarkLiveAsync(Guid id);

        Task MarkPendingUserAsync(Guid livestreamId, Guid userId, DateTimeOffset triggerMinute);

        Task<Livestream> GetLivestreamFromTriggerMinute(Guid userId, DateTimeOffset triggerMinute);

        Task<IEnumerable<Livestream>> GetAvailableLivestreamsAsync();

    }

}
