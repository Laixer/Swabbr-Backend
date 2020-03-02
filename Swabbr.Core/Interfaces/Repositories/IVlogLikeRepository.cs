using Swabbr.Core.Entities;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.Repositories
{
    public interface IVlogLikeRepository : IRepository<VlogLike, VlogLikeId>, ICudFunctionality<VlogLike, VlogLikeId>
    {

        /// <summary>
        /// Returns the count of all given likes by a single user.
        /// </summary>
        /// <param name="userId">Unique identifier of the user who submitted the likes.</param>
        /// <returns></returns>
        Task<int> GetGivenCountForUserAsync(Guid userId);

        Task<IEnumerable<VlogLike>> GetForVlogAsync(Guid vlogId);

        Task<bool> ExistsAsync(VlogLikeId vlogLikeId);

    }
}