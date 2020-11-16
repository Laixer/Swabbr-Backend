using Microsoft.Extensions.Logging;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Interfaces.Services;
using System;
using System.Threading.Tasks;

namespace Swabbr.Core.Services
{
    /// <summary>
    ///     Checks the health of our backend.
    /// </summary>
    public sealed class HealthCheckService : IHealthCheckService
    {
        private readonly ILivestreamService _livestreamService;
        private readonly INotificationService _notificationService;
        private readonly IHealthCheckRepository _healthCheckRepository;
        private readonly ILogger<HealthCheckService> _logger;

        /// <summary>
        ///     Create new instance.
        /// </summary>
        public HealthCheckService(ILivestreamService livestreamService,
            INotificationService notificationService,
            IHealthCheckRepository healthCheckRepository,
            ILogger<HealthCheckService> logger)
        {
            _livestreamService = livestreamService ?? throw new ArgumentNullException(nameof(livestreamService));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _healthCheckRepository = healthCheckRepository ?? throw new ArgumentNullException(nameof(healthCheckRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        ///     Checks the livestream service, notification service and database.
        /// </summary>
        public async Task<bool> IsHealthyAsync()
            => !await _livestreamService.IsServiceOnlineAsync().ConfigureAwait(false) ||
                !await _notificationService.IsServiceOnlineAsync().ConfigureAwait(false) ||
                !await _healthCheckRepository.IsAliveAsync().ConfigureAwait(false);
    }
}
