﻿using Swabbr.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces
{

    public interface ILivestreamRepository : IRepository<Livestream>
    {
        /// <summary>
        /// Returns all active livestreams.
        /// </summary>
        Task<IEnumerable<Livestream>> GetActiveLivestreamsAsync();

        /// <summary>
        /// Returns the amount of inactive livestreams.
        /// </summary>
        /// TODO THOMAS Why would we ever need this? (pool probably?)
        Task<int> GetAvailableLivestreamCountAsync();

        /// <summary>
        /// Returns a <see cref="Livestream"/> that is active and currently claimed by the specified user.
        /// </summary>
        Task<Livestream> GetActiveLivestreamForUserAsync(Guid userId);

        /// <summary>
        /// Returns a <see cref="Livestream"/> that is available for usage and claims ownership for
        /// the specified user.
        /// </summary>
        Task<Livestream> ReserveLivestreamForUserAsync(Guid userId);

        /// <summary>
        /// Return a single entity by providing its unique identifier.
        /// </summary>
        /// <param name="livestreamId">Unique identifier of the livestream</param>
        Task<Livestream> GetByIdAsync(string livestreamId);
    }
}