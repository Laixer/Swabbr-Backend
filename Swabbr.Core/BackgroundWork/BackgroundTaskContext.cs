using System;

namespace Swabbr.Core.BackgroundWork
{
    /// <summary>
    ///     Context for executing background tasks.
    /// </summary>
    /// <remarks>
    ///     This extends <see cref="AppContext"/> and thus can be
    ///     instantiated using an app context factory implementation.
    /// </remarks>
    public class BackgroundTaskContext : AppContext
    {
        /// <summary>
        ///     Task id.
        /// </summary>
        public Guid TaskId { get; set; }

        /// <summary>
        ///     Value to be processed by the task.
        /// </summary>
        public object Value { get; set; }
    }
}
