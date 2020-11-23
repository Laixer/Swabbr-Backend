using Swabbr.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.Repositories
{
    /// <summary>
    /// Repository for Vlog entities.
    /// </summary>
    public interface IVlogRepository : IRepository<Vlog, Guid>
    {
        /// <summary>
        ///     Adds a view to a vlog.
        /// </summary>
        /// <param name="vlogId">The vlog that has been watched.</param>
        Task AddView(Guid vlogId);

        /// <summary>
        ///     Creates a new vlog in our data store.
        /// </summary>
        /// <param name="entity">The created vlog.</param>
        /// <returns>The created and re-fetched vlog.</returns>
        Task<Vlog> CreateAsync(Vlog entity);

        /// <summary>
        ///     Soft deletes a vlog in our data store.
        /// </summary>
        /// <param name="id">The vlog to delete.</param>
        Task DeleteAsync(Guid id);

        /// <summary>
        ///     Returns whether the vlog with the specified id exists.
        /// </summary>
        /// <param name="vlogId">Unique identifier of the vlog.</param>
        Task<bool> ExistsAsync(Guid vlogId);

        /// <summary>
        ///     Returns a collection of featured vlogs.
        /// </summary>
        Task<IEnumerable<Vlog>> GetFeaturedVlogsAsync();

        /// <summary>
        ///     Gets a collection of most recent vlogs for a user.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="maxCount"></param>
        /// <returns></returns>
        Task<IEnumerable<Vlog>> GetMostRecentVlogsForUserAsync(Guid userId, uint maxCount);

        /// <summary>
        ///     Returns a collection of vlogs that are 
        ///     owned by the specified user.
        /// </summary>
        /// <param name="userId">Owner user id.</param>
        Task<IEnumerable<Vlog>> GetVlogsFromUserAsync(Guid userId);

        /// <summary>
        ///     Updates a vlog in our data store.
        /// </summary>
        /// <param name="entity">The vlog with updated parameters.</param>
        Task UpdateAsync(Vlog entity);
    }
}
