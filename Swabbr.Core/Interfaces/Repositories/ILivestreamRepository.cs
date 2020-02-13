using Swabbr.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces
{

    /// <summary>
    /// Repository for <see cref="Livestream"/> entities.
    /// </summary>
    public interface ILivestreamRepository : IRepository<Livestream>
    {

        /// <summary>
        /// Returns all active livestreams.
        /// </summary>
        Task<IEnumerable<Livestream>> GetActiveLivestreamsAsync();

        /// <summary>
        /// Returns a <see cref="Livestream"/> that is active and currently claimed by the specified user.
        /// </summary>
        /// <param name="userId">The internal <see cref="User"/> id</param>
        Task<Livestream> GetActiveLivestreamForUserAsync(Guid userId);

        /// <summary>
        /// Returns a <see cref="Livestream"/> that is available for usage and claims ownership for
        /// the specified user.
        /// </summary>
        /// <param name="userId">The internal <see cref="User"/> id</param>
        Task<Livestream> ReserveLivestreamForUserAsync(Guid userId);

        /// <summary>
        /// Return a single entity by providing its unique identifier.
        /// </summary>
        /// <param name="livestreamId">Unique identifier of the livestream</param>
        Task<Livestream> GetByIdAsync(string livestreamId);

    }

}
