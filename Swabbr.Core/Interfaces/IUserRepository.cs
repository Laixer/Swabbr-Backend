using Swabbr.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces
{
    /// <summary>
    /// Repository for User entities.
    /// </summary>
    public interface IUserRepository : IRepository<User>
    {
        /// <summary>
        /// Get a single user entity by id.
        /// </summary>
        Task<User> GetByIdAsync(Guid userId);

        /// <summary>
        /// Get a single user entity by its email address.
        /// </summary>
        Task<User> GetByEmailAsync(string email);

        /// <summary>
        /// Searching for users.
        /// </summary>
        /// <param name="query">Search query to run against the user properties.</param>
        /// <param name="offset">Result record offset.</param>
        /// <param name="limit">Result limit.</param>
        /// <returns>A collection of users matching the search query.</returns>
        Task<IEnumerable<User>> SearchAsync(string query, uint offset, uint limit);

        //TODO: Remove
        Task TempDeleteTables();
    }
}