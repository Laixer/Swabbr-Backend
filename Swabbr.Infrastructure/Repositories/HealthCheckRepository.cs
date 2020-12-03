using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Infrastructure.Abstractions;
using System.Threading.Tasks;

namespace Swabbr.Infrastructure.Repositories
{
    /// <summary>
    ///     Repository to check our database health.
    /// </summary>
    internal class HealthCheckRepository : RepositoryBase, IHealthCheckRepository
    {
        /// <summary>
        ///     Checks if the database is online.
        /// </summary>
        public async Task<bool> IsAliveAsync()
        {
            var sql = "SELECT 1;";

            await using var context = await CreateNewDatabaseContext(sql);

            await context.ScalarAsync<long>();

            return true;
        }
    }
}
