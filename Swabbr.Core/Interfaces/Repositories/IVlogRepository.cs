using Swabbr.Core.Entities;
using Swabbr.Core.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.Repositories
{
    /// <summary>
    /// Repository for Vlog entities.
    /// </summary>
    public interface IVlogRepository : IRepository<Vlog, Guid>, ICudFunctionality<Vlog, Guid>
    {
        /// <summary>
        /// Returns a collection of vlogs that are owned by the specified user.
        /// </summary>
        /// <param name="userId">Unique identifier of the user and owner of the vlogs.</param>
        Task<IEnumerable<Vlog>> GetVlogsByUserAsync(Guid userId);

        /// <summary>
        /// Returns a collection of featured vlogs.
        /// </summary>
        Task<IEnumerable<Vlog>> GetFeaturedVlogsAsync();

        /// <summary>
        /// Returns whether the vlog with the specified id exists.
        /// </summary>
        /// <param name="vlogId">Unique identifier of the vlog.</param>
        Task<bool> ExistsAsync(Guid vlogId);

        /// <summary>
        /// Returns the id's of the users the specified vlog is currently shared with.
        /// </summary>
        /// <param name="vlogId">Unique identifier of the vlog.</param>
        /// <returns></returns>
        Task<IEnumerable<Guid>> GetSharedUserIdsAsync(Guid vlogId);

        /// <summary>
        /// Adds the specified user as a shared user for the specified vlog.
        /// </summary>
        /// <param name="vlogId">Unique identifier of the vlog.</param>
        /// <param name="userId">Unique identifier of the user to share the vlog with.</param>
        /// <returns></returns>
        Task ShareWithUserAsync(Guid vlogId, Guid userId);

        /// <summary>
        /// Returns the amount of vlogs that a user has created.
        /// </summary>
        /// <param name="userId">Unique identifier of the user to check the amount of vlogs for.</param>
        /// <returns></returns>
        Task<int> GetVlogCountForUserAsync(Guid userId);

        /// <summary>
        /// Checks to see if there is a <see cref="Vlog"/> that belongs to a
        /// specified <see cref="Livestream"/>.
        /// </summary>
        /// <param name="livestreamId">Internal <see cref="Livestream"/> id</param>
        /// <returns><see cref="true"/> if exists</returns>
        Task<bool> ExistsForLivestreamAsync(Guid livestreamId);

        Task<Vlog> GetVlogFromLivestreamAsync(Guid livestreamId);

    }
}