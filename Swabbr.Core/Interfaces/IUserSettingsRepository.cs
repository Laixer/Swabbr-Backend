﻿using Swabbr.Core.Entities;
using System;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces
{
    /// <summary>
    /// Repository for User Settings.
    /// </summary>
    public interface IUserSettingsRepository : IRepository<UserSettings>
    {
        /// <summary>
        /// Get the user settings for the user with the specified id
        /// </summary>
        /// <param name="userId">Unique identifier of the user to which the settings belong</param>
        /// <returns></returns>
        Task<UserSettings> GetByUserId(Guid userId);
    }
}