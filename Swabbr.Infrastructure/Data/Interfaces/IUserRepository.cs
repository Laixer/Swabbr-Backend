using Swabbr.Core.Interfaces;
using Swabbr.Infrastructure.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swabbr.Infrastructure.Data.Interfaces
{
    public interface IUserRepository : IRepository<UserEntity>
    {
        /// <summary>
        /// Searching for users.
        /// </summary>
        /// <param name="query">Search query to run against the user properties.</param>
        /// <param name="offset">Result record offset.</param>
        /// <param name="limit">Result limit.</param>
        /// <returns>A collection of users matching the search query.</returns>
        Task<IEnumerable<UserEntity>> SearchAsync(string query, uint offset, uint limit);

        //TODO: Remove
        Task TempDeleteTables();
    }
}
