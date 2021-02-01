using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Swabbr.Core.Abstractions
{
    /// <summary>
    ///     Generic timed hosted service. Use this to perform
    ///     periodic background work.
    /// </summary>
    /// <remarks>
    ///     Modify <see cref="Interval"/> to determine the
    ///     interval for the periodic work.
    /// </remarks>
    public abstract class PeriodicHostedService : IHostedService, IDisposable
    {
        private int executionCount = 0;
        private readonly ILogger _logger;
        protected TimeSpan Interval = TimeSpan.FromMinutes(5);
        private Timer _timer;

        /// <summary>
        ///     Only the base should be able to modify the count.
        ///     Other classes may access the count.
        /// </summary>
        public int ExecutionCount => executionCount;

        /// <summary>
        ///     Create new instance.
        /// </summary>
        public PeriodicHostedService(ILogger logger)
            => _logger = logger;

        /// <summary>
        ///     Start the periodic background work.
        /// </summary>
        /// <param name="stoppingToken">Stopping token.</param>
        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service running.");

            _timer = new Timer(DoWork, stoppingToken, TimeSpan.Zero, Interval);

            return Task.CompletedTask;
        }

        /// <summary>
        ///     The function that will be called periodically, which
        ///     in turn calls <see cref="DoPeriodicWork(CancellationToken)"/>.
        /// </summary>
        /// <param name="state">Stopping token.</param>
        private void DoWork(object state)
        {
            Interlocked.Increment(ref executionCount);

            _logger.LogTrace($"Periodic hosted service running execution {ExecutionCount}");

            var token = (CancellationToken)state;

            DoPeriodicWork(token);
        }

        /// <summary>
        ///     This function will be called periodically. Override this
        ///     function to determine the work that has to be done.
        /// </summary>
        /// <remarks>
        ///     The <paramref name="stoppingToken"/> is the same token 
        ///     which is used when invoking this hosted service.
        /// </remarks>
        /// <param name="stoppingToken">Stopping token.</param>
        protected abstract void DoPeriodicWork(CancellationToken stoppingToken);

        /// <summary>
        ///     Stop the periodic work.
        /// </summary>
        /// <param name="stoppingToken">Cancellation token.</param>
        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        /// <summary>
        ///     Called on graceful shutdown.
        /// </summary>
        public virtual void Dispose()
            => _timer?.Dispose();
    }
}
