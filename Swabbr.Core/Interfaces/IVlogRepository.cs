using Swabbr.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces
{
    /// <summary>
    /// Repository for Vlog entities.
    /// </summary>
    public interface IVlogRepository : IRepository<Vlog>
    {
        /// <summary>
        /// Returns a collection of vlogs that are owned by the specified user.
        /// </summary>
        /// <param name="userId">Unique identifier of the user and owner of the vlogs.</param>
        Task<IEnumerable<Vlog>> GetVlogsByUserAsync(Guid userId);

        /// <summary>
        /// Returns a single vlog with the specified Id.
        /// </summary>
        /// <param name="vlogId">Unique identifier of the vlog.</param>
        Task<Vlog> GetByIdAsync(Guid vlogId);

        /// <summary>
        /// Returns whether the vlog with the specified id exists.
        /// </summary>
        /// <param name="vlogId">Unique identifier of the vlog.</param>
        Task<bool> ExistsAsync(Guid vlogId);

        /// <summary>
        /// Returns the amount of vlogs that a user has created.
        /// </summary>
        /// <param name="userId">Unique identifier of the user to check the amount of vlogs for.</param>
        /// <returns></returns>
        Task<int> GetVlogCountForUserAsync(Guid userId);
    }
}