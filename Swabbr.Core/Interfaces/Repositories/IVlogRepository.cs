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

        Task AddView(Guid vlogId);

        Task<Vlog> CreateAsync(Vlog entity);

        /// <summary>
        /// Returns whether the vlog with the specified id exists.
        /// </summary>
        /// <param name="vlogId">Unique identifier of the vlog.</param>
        Task<bool> ExistsAsync(Guid vlogId);

        /// <summary>
        /// Checks to see if there is a <see cref="Vlog"/> that belongs to a
        /// specified <see cref="Livestream"/>.
        /// </summary>
        /// <param name="livestreamId">Internal <see cref="Livestream"/> id</param>
        /// <returns>True if exists</returns>
        Task<bool> ExistsForLivestreamAsync(Guid livestreamId);

        /// <summary>
        /// Returns a collection of featured vlogs.
        /// </summary>
        Task<IEnumerable<Vlog>> GetFeaturedVlogsAsync();

        Task<IEnumerable<Vlog>> GetMostRecentVlogsForUserAsync(Guid userId, uint maxCount);

        /// <summary>
        /// Returns the amount of vlogs that a user has created.
        /// </summary>
        /// <param name="userId">Unique identifier of the user to check the amount of vlogs for.</param>
        /// <returns></returns>
        Task<int> GetVlogCountForUserAsync(Guid userId);

        Task<Vlog> GetVlogFromLivestreamAsync(Guid livestreamId);

        Task<Vlog> GetVlogFromReactionAsync(Guid reactionId);

        /// <summary>
        /// Returns a collection of vlogs that are owned by the specified user.
        /// </summary>
        /// <param name="userId">Unique identifier of the user and owner of the vlogs.</param>
        Task<IEnumerable<Vlog>> GetVlogsFromUserAsync(Guid userId);

        Task HardDeleteAsync(Guid id);

        Task SoftDeleteAsync(Guid id);

        Task<Vlog> UpdateAsync(Vlog entity);

    }

}