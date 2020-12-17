using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Infrastructure.Abstractions;
using System.Threading.Tasks;

namespace Swabbr.Infrastructure.Repositories
{
    /// <summary>
    ///     Repository to check our database health.
    /// </summary>
    internal class HealthCheckRepository : DatabaseContextBase, IHealthCheckRepository
    {
        /// <summary>
        ///     Checks if the database is online.
        /// </summary>
        /// <remarks>
        ///     This simply tries to "SELECT 1".
        /// </remarks>
        public async Task TestServiceAsync()
        {
            var sql = "SELECT 1;";

            await using var context = await CreateNewDatabaseContext(sql);

            await context.ScalarAsync<int>();
        }
    }
}
