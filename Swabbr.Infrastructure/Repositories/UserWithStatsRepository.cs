using Dapper;
using Laixer.Infra.Npgsql;
using Laixer.Utility.Extensions;
using Swabbr.Core.Entities;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using static Swabbr.Infrastructure.Database.DatabaseConstants;

namespace Swabbr.Infrastructure.Repositories
{

    /// <summary>
    /// Repository for <see cref="SwabbrUserWithStats"/> entities.
    /// </summary>
    public sealed class UserWithStatsRepository : IUserWithStatsRepository
    {

        private readonly IDatabaseProvider _databaseProvider;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public UserWithStatsRepository(IDatabaseProvider databaseProvider)
        {
            _databaseProvider = databaseProvider ?? throw new ArgumentNullException(nameof(databaseProvider));
        }

        /// <summary>
        /// Gets a single <see cref="SwabbrUserWithStats"/> from the database.
        /// </summary>
        /// <param name="id"><see cref="SwabbrUserWithStats"/> internal id</param>
        /// <returns><see cref="SwabbrUserWithStats"/></returns>
        public async Task<SwabbrUserWithStats> GetAsync(Guid id)
        {
            id.ThrowIfNullOrEmpty();
            using (var connection = _databaseProvider.GetConnectionScope())
            {
                var sql = $"SELECT * FROM {ViewUserWithStats} WHERE id = '{id}';";
                var result = await connection.QueryAsync<SwabbrUserWithStats>(sql).ConfigureAwait(false);
                if (result == null || !result.Any()) { throw new EntityNotFoundException($"Could not find User with id = {id}"); }
                else
                {
                    return result.First();
                }
            }
        }

        /// <summary>
        /// Searches for <see cref="SwabbrUserWithStats"/> in the database.
        /// </summary>
        /// <param name="searchString">Search string</param>
        /// <param name="page">Page number, default is 1</param>
        /// <param name="itemsPerPage">Items per page, default is 50</param>
        /// <returns><see cref="IEnumerable{SwabbrUserWithStats}"/></returns>
        public async Task<IEnumerable<SwabbrUserWithStats>> SearchAsync(string searchString, int page = 1, int itemsPerPage = 50)
        {
            searchString.ThrowIfNullOrEmpty();
            if (page < 1) { throw new ArgumentOutOfRangeException("Page number must be greater than one"); }
            if (itemsPerPage < 1) { throw new ArgumentOutOfRangeException("Items per page must be greater than one"); }

            using (var connection = _databaseProvider.GetConnectionScope())
            {
                var sql = $"SELECT * FROM {ViewUserWithStats}" +
                    $" WHERE nickname LIKE '%{searchString}%'" +
                    $" OFFSET {(page - 1) * itemsPerPage}" +
                    $" LIMIT {itemsPerPage};"; // TODO Query builder or something
                return await connection.QueryAsync<SwabbrUserWithStats>(sql).ConfigureAwait(false);
            }
        }

        public Task<IEnumerable<SwabbrUserWithStats>> ListFollowersAsync(Guid id, int page, int itemsPerPage) => throw new NotImplementedException();

        public Task<IEnumerable<SwabbrUserWithStats>> ListFollowingAsync(Guid id, int page, int itemsPerPage) => throw new NotImplementedException();

    }

}
