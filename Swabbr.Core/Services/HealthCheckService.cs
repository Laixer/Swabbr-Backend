using Microsoft.Extensions.Logging;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Interfaces.Services;
using System;
using System.Threading.Tasks;

namespace Swabbr.Core.Services
{
    /// <summary>
    /// Checks the health of our backend.
    /// </summary>
    public sealed class HealthCheckService : IHealthCheckService
    {
        private readonly ILivestreamService _livestreamService;
        private readonly INotificationService _notificationService;
        private readonly IDatabaseProvider _databaseProvider;
        private readonly ILogger logger;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public HealthCheckService(ILivestreamService livestreamService,
            INotificationService notificationService,
            IDatabaseProvider databaseProvider,
            ILoggerFactory loggerFactory)
        {
            _livestreamService = livestreamService ?? throw new ArgumentNullException(nameof(livestreamService));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _databaseProvider = databaseProvider ?? throw new ArgumentNullException(nameof(databaseProvider));
            logger = (loggerFactory != null) ? loggerFactory.CreateLogger(nameof(HealthCheckService)) : throw new ArgumentNullException(nameof(loggerFactory));
        }

        /// <summary>
        /// Checks the livestream service, notification service and database.
        /// </summary>
        /// <returns><see cref="bool"/> result</returns>
        public async Task<bool> IsHealthyAsync()
        {
            if (!await _livestreamService.IsServiceOnlineAsync().ConfigureAwait(false)) { return false; }
            if (!await _notificationService.IsServiceOnlineAsync().ConfigureAwait(false)) { return false; }
            if (!await IsDatabaseOnlineAsync().ConfigureAwait(false)) { return false; }

            return true;
        }

        /// <summary>
        /// Checks if our database is online.
        /// </summary>
        /// <returns><see cref="bool"/> result</returns>
        private async Task<bool> IsDatabaseOnlineAsync()
        {
            try
            {
                using var connection = _databaseProvider.GetConnectionScope();
                var sql = $"SELECT 1 FROM public.user"; // TODO This might be too specific?
                await connection.QueryAsync(sql).ConfigureAwait(false);
                return true;
            }
            catch (Exception e)
            {
                logger.LogError("Could not connect to database", e.Message);
                return false;
            }
        }
    }
}
