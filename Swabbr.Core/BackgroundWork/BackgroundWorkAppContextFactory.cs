using Swabbr.Core.Interfaces.Factories;
using System;

namespace Swabbr.Core.BackgroundWork
{
    /// <summary>
    ///     Factory for creating application context for
    ///     hosted services to use.
    /// </summary>
    public class BackgroundWorkAppContextFactory : IAppContextFactory
    {
        private readonly IServiceProvider _services;

        /// <summary>
        ///     Create new instance.
        /// </summary>
        public BackgroundWorkAppContextFactory(IServiceProvider services) 
            => _services = services ?? throw new ArgumentNullException(nameof(services));

        /// <summary>
        ///     Generates a new <see cref="BackgroundTaskContext"/> entity.
        /// </summary>
        /// <remarks>
        ///     This assigns a cancellation token which 
        ///     upon which cancellation will be requested
        ///     when this factory object is disposed.
        /// </remarks>
        /// <returns>App context object.</returns>
        public Core.AppContext CreateAppContext()
            => new BackgroundTaskContext
            {
                ServiceProvider = _services,
            };
    }
}
