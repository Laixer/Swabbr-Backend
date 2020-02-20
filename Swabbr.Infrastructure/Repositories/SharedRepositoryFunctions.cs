using Dapper;
using Laixer.Infra.Npgsql;
using Laixer.Utility.Extensions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Swabbr.Infrastructure.Repositories
{

    /// <summary>
    /// Contains reusable repository functionality.
    /// </summary>
    internal static class SharedRepositoryFunctions
    {

        /// <summary>
        /// Checks if an entity exists in our database.
        /// TODO Sending table string might be dangerous?
        /// </summary>
        /// <param name="provider"><see cref="IDatabaseProvider"/></param>
        /// <param name="table">Table name in our database</param>
        /// <param name="id">Internal entity id</param>
        /// <returns><see cref="true"/> if the entity exists</returns>
        internal static async Task<bool> ExistsAsync(IDatabaseProvider provider, string table, Guid id)
        {
            if (provider == null) { throw new ArgumentNullException(nameof(provider)); }
            table.ThrowIfNullOrEmpty();
            id.ThrowIfNullOrEmpty();

            using (var connection = provider.GetConnectionScope())
            {
                var sql = $"SELECT 1 FROM {table} WHERE id = '{id}';";
                var result = await connection.QueryAsync<int>(sql).ConfigureAwait(false);
                if (result == null) { throw new InvalidOperationException("Result for exist checks should never be null"); }
                if (result.Count() > 1) { throw new InvalidOperationException("Single check result should never have more than one entity"); }
                return result.Count() == 1;
            }
        }

    }

}
