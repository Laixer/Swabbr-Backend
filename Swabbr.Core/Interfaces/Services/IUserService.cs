﻿using Swabbr.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.Services
{
    public interface IUserService
    {
        /// <summary>
        /// Checks if a user with the given id exists.
        /// </summary>
        /// TODO THOMAS This should never be required (getasync will just return null --> throw exception)
        Task<bool> ExistsAsync(Guid userId);

        /// <summary>
        /// Get a single user entity by id.
        /// </summary>
        Task<SwabbrUser> GetAsync(Guid userId);

        /// <summary>
        /// Update a user entity.
        /// </summary>
        Task<SwabbrUser> UpdateAsync(SwabbrUser user);

        /// <summary>
        /// Get a single user entity by its email address.
        /// </summary>
        Task<SwabbrUser> GetByEmailAsync(string email);

        /// <summary>
        /// Create a new user.
        /// </summary>
        /// <param name="user">User to create.</param>
        Task<SwabbrUser> CreateAsync(SwabbrUser user);

        /// <summary>
        /// Search for application users based on a given search query.
        /// </summary>
        /// <param name="query">Search query to run against the user properties.</param>
        /// <param name="offset">Result record offset.</param>
        /// <param name="limit">Result limit.</param>
        /// <returns>A collection of users matching the search query.</returns>
        Task<IEnumerable<SwabbrUser>> SearchAsync(string query, uint offset, uint limit);
    }
}