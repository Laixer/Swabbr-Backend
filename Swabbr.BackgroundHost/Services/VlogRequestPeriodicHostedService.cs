using Microsoft.Extensions.Logging;
using Swabbr.BackgroundHost.Abstraction;
using Swabbr.BackgroundHost.BackgroundTasks;
using Swabbr.Core.BackgroundWork;
using System;
using System.Threading;

namespace Swabbr.BackgroundHost.Services
{
    /// <summary>
    ///     Periodic vlog request hosted service which triggers
    ///     every minute to send all required vlog requests.
    /// </summary>
    internal class VlogRequestPeriodicHostedService : PeriodicHostedService
    {
        private readonly DispatchManager _dispatchManager;

        /// <summary>
        ///     Create new instance.
        /// </summary>
        public VlogRequestPeriodicHostedService(DispatchManager dispatchManager,
            ILogger<PeriodicHostedService> logger)
            : base(logger)
        {
            _dispatchManager = dispatchManager ?? throw new ArgumentNullException(nameof(dispatchManager));

            // Override the timer interval.
            Interval = TimeSpan.FromMinutes(1);
        }

        /// <summary>
        ///     Executes a vlog record request for all users that
        ///     belong to the DateTimeOffset.Now minute.
        /// </summary>
        /// <param name="stoppingToken">Cancellation token.</param>
        protected override void DoPeriodicWork(CancellationToken stoppingToken)
        {
            var now = DateTimeOffset.UtcNow;
            var minute = new DateTimeOffset(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0, TimeSpan.Zero);

            _dispatchManager.Dispatch<VlogRequestCycleBackgroundTask>(minute);
        }
    }
}
