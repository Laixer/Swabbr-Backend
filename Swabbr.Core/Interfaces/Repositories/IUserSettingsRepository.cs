using Swabbr.Core.Entities;
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
        Task<UserSettings> GetForUserAsync(Guid userId);

        /// <summary>
        /// Returns whether a user settings entity for a given user exists
        /// </summary>
        /// <param name="userId">Unique identifier of the user to which the settings should belong</param>
        /// <returns></returns>
        /// TODO THOMAS Why can there even be users without user settings? This should never be possible.
        /// TODO THOMAS It might be better to just have the user settings as a view, where all their values are in the users table as well.
        Task<bool> ExistsForUserAsync(Guid userId);
    }
}