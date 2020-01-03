using Swabbr.Core.Entities;
using System;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces
{
    /// <summary>
    /// Repository for Vlog entities.
    /// </summary>
    public interface IVlogRepository : IRepository<Vlog>
    {
        /// <summary>
        /// Returns the amount of vlogs that a user has created.
        /// </summary>
        /// <param name="userId">Unique identifier of the user to check the amount of vlogs for.</param>
        /// <returns></returns>
        Task<int> GetVlogCountForUserAsync(Guid userId);
    }
}