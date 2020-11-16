using Dapper;
using Microsoft.Extensions.Logging;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Infrastructure.Providers;
using System;
using System.Threading.Tasks;

namespace Swabbr.Infrastructure.Repositories
{
    /// <summary>
    ///     Checks if our database is online.
    /// </summary>
    public class HealthCheckRepository : IHealthCheckRepository
    {
        private readonly IDatabaseProvider _databaseProvider;
        private readonly ILogger<HealthCheckRepository> _logger;

        /// <summary>
        ///     Create new instance.
        /// </summary>
        public HealthCheckRepository(IDatabaseProvider databaseProvider,
            ILogger<HealthCheckRepository> logger)
        {
            _databaseProvider = databaseProvider ?? throw new ArgumentNullException(nameof(databaseProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        ///     Checks if our database is online.
        /// </summary>
        public async Task<bool> IsAliveAsync()
        {
            try
            {
                using var connection = _databaseProvider.GetConnectionScope();
                var sql = $"SELECT 1";
                await connection.QueryAsync(sql).ConfigureAwait(false);
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError("Could not connect to database", e.Message);
                return false;
            }
        }
    }
}
