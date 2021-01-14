using Swabbr.Core.Interfaces.Factories;
using System;
using System.Threading;

namespace Swabbr.BackgroundHost
{
    /// <summary>
    ///     Factory for creating application context for
    ///     hosted services to use.
    /// </summary>
    internal class BackgroundHostAppContextFactory : IAppContextFactory, IDisposable
    {
        private readonly IServiceProvider _services;

        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        /// <summary>
        ///     Create new instance.
        /// </summary>
        public BackgroundHostAppContextFactory(IServiceProvider services) 
            => _services = services ?? throw new ArgumentNullException(nameof(services));

        /// <summary>
        ///     Generates a new app context entity.
        /// </summary>
        /// <remarks>
        ///     This assigns a cancellation token which 
        ///     upon which cancellation will be requested
        ///     when this factory object is disposed.
        /// </remarks>
        /// <returns>App context object.</returns>
        public Core.AppContext CreateAppContext()
            => new Core.AppContext
            {
                ServiceProvider = _services,
                CancellationToken = _cts.Token
            };

        /// <summary>
        ///     Called on graceful shutdown.
        /// </summary>
        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
        }
    }
}
