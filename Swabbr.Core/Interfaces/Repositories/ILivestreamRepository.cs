using Swabbr.Core.Entities;
using Swabbr.Core.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.Repositories
{

    /// <summary>
    /// Repository for <see cref="Livestream"/> entities.
    /// </summary>
    public interface ILivestreamRepository : IRepository<Livestream, Guid>, ICudFunctionality<Livestream, Guid>
    {

        /// <summary>
        /// Returns all active livestreams.
        /// </summary>
        Task<IEnumerable<Livestream>> GetActiveLivestreamsAsync();

        /// <summary>
        /// Returns a <see cref="Livestream"/> that is active and currently claimed by the specified user.
        /// </summary>
        /// <param name="userId">The internal <see cref="SwabbrUser"/> id</param>
        Task<Livestream> GetActiveLivestreamForUserAsync(Guid userId);

        /// <summary>
        /// Returns a <see cref="Livestream"/> that is available for usage and claims ownership for
        /// the specified user.
        /// </summary>
        /// <param name="userId">The internal <see cref="SwabbrUser"/> id</param>
        Task<Livestream> ReserveLivestreamForUserAsync(Guid userId);

        /// <summary>
        /// Return a single entity by providing its unique identifier.
        /// </summary>
        /// <param name="livestreamId">Unique identifier of the livestream</param>
        Task<Livestream> GetByIdAsync(string livestreamId);

    }

}
