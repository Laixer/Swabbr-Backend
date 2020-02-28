using Swabbr.Core.Entities;
using Swabbr.Core.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.Repositories
{

    /// <summary>
    /// Repository for <see cref="SwabbrUser"/> entities.
    /// </summary>
    public interface IUserRepository : IRepository<SwabbrUser, Guid>, ICudFunctionality<SwabbrUser, Guid>
    {

        /// <summary>
        /// Gets the <see cref="UserSettings"/> for a single <see cref="SwabbrUser"/>.
        /// </summary>
        /// <param name="userId">Internal <see cref="SwabbrUser"/> id</param>
        /// <returns><see cref="UserSettings"/></returns>
        Task<UserSettings> GetUserSettingsAsync(Guid userId);

        Task<UserStatistics> GetUserStatisticsAsync(Guid userId);

        /// <summary>
        /// Updates our <see cref="UserSettings"/>.
        /// TODO Do we need this?
        /// </summary>
        /// <param name="userSettings"><see cref="UserSettings"/></param>
        /// <returns><see cref="UserSettings"/></returns>
        Task<UserSettings> UpdateUserSettingsAsync(UserSettings userSettings);

        /// <summary>
        /// Get a single <see cref="SwabbrUser"/> entity by its email address.
        /// </summary>
        /// <param name="email">Stored email address</param>
        Task<SwabbrUser> GetByEmailAsync(string email);

        /// <summary>
        /// Allows us to search for a collection of <see cref="SwabbrUser"/>s.
        /// TODO THOMAS Is this the optimal pagination method? Maybe make this a bit more explicit
        /// </summary>
        /// <param name="query">Search query to run against the user properties</param>
        /// <param name="offset">Result record offset</param>
        /// <param name="limit">Result limit</param>
        /// <returns>A collection of users matching the search query</returns>
        Task<IEnumerable<SwabbrUser>> SearchAsync(string query, uint offset, uint limit);

        /// <summary>
        /// Checks if a user with the given id exists.
        /// </summary>
        Task<bool> UserExistsAsync(Guid userId);

        Task<IEnumerable<SwabbrUser>> GetFollowingAsync(Guid userId);

        Task<IEnumerable<SwabbrUser>> GetFollowersAsync(Guid userId);

    }

}
