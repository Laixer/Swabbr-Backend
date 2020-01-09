using Swabbr.Core.Entities;
using System;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces
{
    public interface IVlogLikeRepository : IRepository<VlogLike>
    {
        /// <summary>
        /// Get a like for a vlog given by a specific user
        /// </summary>
        /// <param name="vlogId">Unique identifier of the vlog.</param>
        /// <param name="userId">Unique identifier of the user who submitted the like.</param>
        /// <returns></returns>
        Task<VlogLike> GetByUserIdAsync(Guid vlogId, Guid userId);
    }
}