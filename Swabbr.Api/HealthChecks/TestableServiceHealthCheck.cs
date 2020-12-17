using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Swabbr.Core.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Swabbr.Api.HealthChecks
{
    /// <summary>
    ///     Generic health check for a <see cref="ITestableService"/>.
    /// </summary>
    /// <typeparam name="TTestableService">Type of service to check.</typeparam>
    public class TestableServiceHealthCheck<TTestableService> : IHealthCheck
        where TTestableService : ITestableService
    {
        private readonly TTestableService _testableService;
        private readonly ILogger<TestableServiceHealthCheck<TTestableService>> _logger;

        /// <summary>
        ///     Create new instance.
        /// </summary>
        public TestableServiceHealthCheck(TTestableService testableService,
            ILogger<TestableServiceHealthCheck<TTestableService>> logger)
        {
            _testableService = testableService ?? throw new ArgumentNullException(nameof(testableService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        ///     Checks if our repositories can be reached, meaning our 
        ///     data store is online.
        /// </summary>
        /// <remarks>
        ///     This could be executed in a try/catch block, but the
        ///     ASP HealthCheck framework catches any exceptions and
        ///     logs them for us. If we ever require custom error logs
        ///     we can implement that functionality here using try/catch.
        /// </remarks>
        /// <param name="context">Health check context.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Result of our check.</returns>
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            await _testableService.TestServiceAsync();

            return HealthCheckResult.Healthy();
        }
    }
}
