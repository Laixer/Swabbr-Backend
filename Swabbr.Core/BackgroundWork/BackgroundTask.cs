using System.Threading.Tasks;

namespace Swabbr.Core.BackgroundWork
{
    /// <summary>
    ///     Base for an executable background task.
    /// </summary>
    /// <remarks>
    ///     Dependency injection can be used when 
    ///     implementing this class.
    /// </remarks>
    public abstract class BackgroundTask
    {
        /// <summary>
        ///     Determines if this task is capable of 
        ///     processing a given object.
        /// </summary>
        /// <param name="value">The object to check.</param>
        public abstract bool CanHandle(object value);

        /// <summary>
        ///     Represents the work to be executed by this task.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public virtual Task ExecuteAsync(BackgroundTaskContext context)
            => Task.CompletedTask;
    }
}
