﻿using Swabbr.Core.Entities;
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
        /// TODO THOMAS Why do we need this?
        Task<VlogLike> GetSingleForUserAsync(Guid vlogId, Guid userId);

        /// <summary>
        /// Returns the count of all given likes by a single user.
        /// </summary>
        /// <param name="userId">Unique identifier of the user who submitted the likes.</param>
        /// <returns></returns>
        /// TODO THOMAS Change name, this is a bit vague
        Task<int> GetGivenCountForUserAsync(Guid userId);
    }
}