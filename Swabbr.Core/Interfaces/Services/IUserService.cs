﻿using Swabbr.Core.Entities;
using System;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.Services
{

    /// <summary>
    /// Contract for <see cref="SwabbrUser"/> related operations.
    /// </summary>
    public interface IUserService
    {

        /// <summary>
        /// Checks if a user with the given id exists.
        /// </summary>
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

        Task<UserSettings> GetSettingsAsync(Guid userId);

        Task UpdateSettingsAsync(UserSettings userSettings);

    }

}
