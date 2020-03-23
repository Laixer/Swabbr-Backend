using Swabbr.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.Repositories
{

    /// <summary>
    /// Contract for a <see cref="SwabbrUserWithStats"/> repository.
    /// </summary>
    public interface IUserWithStatsRepository : IRepository<SwabbrUserWithStats, Guid>
    {

        Task<IEnumerable<SwabbrUserWithStats>> SearchAsync(string searchString, int page, int itemsPerPage);

        Task<IEnumerable<SwabbrUserWithStats>> ListFollowingAsync(Guid id, int page, int itemsPerPage);

        Task<IEnumerable<SwabbrUserWithStats>> ListFollowersAsync(Guid id, int page, int itemsPerPage);

        Task<IEnumerable<SwabbrUserWithStats>> GetFromIdsAsync(IEnumerable<Guid> userIds);

    }

}
