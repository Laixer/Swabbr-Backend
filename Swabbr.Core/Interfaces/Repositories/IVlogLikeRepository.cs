using Swabbr.Core.Entities;
using Swabbr.Core.Interfaces.Repositories;
using System;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.Repositories
{
    public interface IVlogLikeRepository : IRepository<VlogLike>
    {
        /// <summary>
        /// Get a like for a vlog given by a specific user
        /// </summary>
        /// <param name="vlogId">Unique identifier of the vlog.</param>
        /// <param name="userId">Unique identifier of the user who submitted the like.</param>
        /// <returns></returns>
        Task<VlogLike> GetSingleForUserAsync(Guid vlogId, Guid userId);

        /// <summary>
        /// Returns the count of all given likes by a single user.
        /// </summary>
        /// <param name="userId">Unique identifier of the user who submitted the likes.</param>
        /// <returns></returns>
        Task<int> GetGivenCountForUserAsync(Guid userId);
    }
}