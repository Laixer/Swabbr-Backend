﻿using Swabbr.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces
{
    public interface IUserRepository : IRepository<UserItem> {
        /// <summary>
        /// Searching for users.
        /// </summary>
        /// <param name="query">Search query to run against the user properties.</param>
        /// <param name="offset">Result record offset.</param>
        /// <param name="limit">Result limit.</param>
        /// <returns>A collection of users matching the search query.</returns>
        Task<IEnumerable<UserItem>> SearchAsync(string query, uint offset, uint limit);
    }
}
