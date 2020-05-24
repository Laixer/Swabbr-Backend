using Swabbr.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.Services
{

    /// <summary>
    /// Contract for <see cref="SwabbrUserWithStats"/> related operations.
    /// </summary>
    public interface IUserWithStatsService
    {

        Task<IEnumerable<SwabbrUserWithStats>> GetFromIdsAsync(IEnumerable<Guid> userIds);

    }

}
