using System;
using System.Threading;

namespace Swabbr.Core.BackgroundWork
{
    /// <summary>
    ///     Context for executing background tasks.
    /// </summary>
    public class BackgroundTaskContext
    {
        /// <summary>
        ///     Task id.
        /// </summary>
        public Guid Id { get; init; }

        /// <summary>
        ///     The cancellation token.
        /// </summary>
        public CancellationToken CancellationToken { get; init; }

        /// <summary>
        ///     Value to be processed by the task.
        /// </summary>
        public object Value { get; init; }

        /// <summary>
        ///     Create new instance.
        /// </summary>
        /// <remarks>
        ///     This assigns <see cref="Id"/>.
        /// </remarks>
        public BackgroundTaskContext()
            => Id = Guid.NewGuid();
    }
}
