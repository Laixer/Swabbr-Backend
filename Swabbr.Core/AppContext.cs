using System;
using System.Threading;

namespace Swabbr.Core
{
    /// <summary>
    ///     Application context.
    /// </summary>
    public class AppContext
    {
        /// <summary>
        ///     Used to signal when an operation should be aborted.
        /// </summary>
        public CancellationToken CancellationToken { get; set; } = CancellationToken.None;

        /// <summary>
        ///     Service provider for the service container.
        /// </summary>
        public IServiceProvider ServiceProvider { get; set; }

        /// <summary>
        ///     User id that belongs to the context.
        /// </summary>
        public Guid UserId { get; set; }
    }
}
