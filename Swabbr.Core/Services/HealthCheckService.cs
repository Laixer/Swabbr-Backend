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
        private readonly INotificationService _notificationService;
        private readonly IHealthCheckRepository _healthCheckRepository;

        /// <summary>
        ///     Create new instance.
        /// </summary>
        public HealthCheckService(INotificationService notificationService,
            IHealthCheckRepository healthCheckRepository)
        {
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _healthCheckRepository = healthCheckRepository ?? throw new ArgumentNullException(nameof(healthCheckRepository));
        }

        /// <summary>
        ///     Checks our database health.
        /// </summary>
        public Task<bool> IsDataStoreHealthyAsync()
            => _healthCheckRepository.IsAliveAsync();

        /// <summary>
        ///     Checks the notification service and database.
        /// </summary>
        public async Task<bool> IsHealthyAsync()
            => await IsDataStoreHealthyAsync().ConfigureAwait(false) &&
               await IsNotificationServiceHealthyAsync().ConfigureAwait(false);

        /// <summary>
        ///     Checks our notification service health.
        /// </summary>
        public Task<bool> IsNotificationServiceHealthyAsync()
            => _notificationService.IsServiceOnlineAsync();
    }
}
