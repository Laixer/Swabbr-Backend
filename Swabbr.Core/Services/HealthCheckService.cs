using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Interfaces.Services;
using System;
using System.Threading.Tasks;

namespace Swabbr.Core.Services
{
    /// <summary>
    ///     Checks the health of our backend.
    /// </summary>
    public class HealthCheckService : IHealthCheckService
    {
        protected readonly INotificationService _notificationService;
        protected readonly IHealthCheckRepository _healthCheckRepository;

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
        public virtual Task<bool> IsDataStoreHealthyAsync()
            => _healthCheckRepository.IsAliveAsync();

        /// <summary>
        ///     Checks the notification service and database.
        /// </summary>
        public virtual async Task<bool> IsHealthyAsync()
            => await IsDataStoreHealthyAsync() &&
               await IsNotificationServiceHealthyAsync();

        /// <summary>
        ///     Checks our notification service health.
        /// </summary>
        public virtual Task<bool> IsNotificationServiceHealthyAsync()
            => _notificationService.IsServiceOnlineAsync();
    }
}
