using Swabbr.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces
{
    /// <summary>
    /// Repository for User entities.
    /// </summary>
    public interface IUserRepository : IRepository<SwabbrUser>
    {
        /// <summary>
        /// Get a single user entity by id.
        /// </summary>
        Task<SwabbrUser> GetAsync(Guid userId);

        /// <summary>
        /// Get a single user entity by its email address.
        /// </summary>
        Task<SwabbrUser> GetByEmailAsync(string email);

        /// <summary>
        /// Searching for users.
        /// </summary>
        /// <param name="query">Search query to run against the user properties.</param>
        /// <param name="offset">Result record offset.</param>
        /// <param name="limit">Result limit.</param>
        /// <returns>A collection of users matching the search query.</returns>
        /// Is this the optimal pagination method? Maybe make this a bit more explicit
        Task<IEnumerable<SwabbrUser>> SearchAsync(string query, uint offset, uint limit);

        /// <summary>
        /// Checks if a user with the given id exists.
        /// </summary>
        Task<bool> UserExistsAsync(Guid userId);
    }
}