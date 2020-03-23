using Swabbr.Core.Entities;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.Repositories
{

    /// <summary>
    /// Contract for a <see cref="VlogLike"/> repository.
    /// </summary>
    public interface IVlogLikeRepository : IRepository<VlogLike, VlogLikeId>, ICudFunctionality<VlogLike, VlogLikeId>
    {

        Task<bool> ExistsAsync(VlogLikeId vlogLikeId);

        Task<IEnumerable<VlogLike>> GetForVlogAsync(Guid vlogId);

        Task<int> GetCountForVlogAsync(Guid vlogId);

    }
}