using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swabbr.Core.Extensions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Swabbr.Core.BackgroundWork
{
    // TODO What to do with preventing a task overflow? Do we even need to since we have no synchronous tasks?
    /// <summary>
    ///     Manager for dispatching background tasks.
    /// </summary>
    /// <remarks>
    ///     This currently only supports asynchronous work.
    /// </remarks>
    public class DispatchManager
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<DispatchManager> _logger;

        /// <summary>
        ///     Create new instance.
        /// </summary>
        public DispatchManager(ILogger<DispatchManager> logger,
            IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
        }

        /// <summary>
        ///     Dispatch a task to be executed in the background.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         The <typeparamref name="TBackgroundTask"/> is 
        ///         expected to be able to handle <paramref name="value"/>.
        ///     </para>
        ///     <para>
        ///         A cancellation token will be assigned during execution
        ///         which will cancel after a timeout. There is no other
        ///         way of cancelling the dispatched operation.
        ///     </para>
        /// </remarks>
        /// <typeparam name="TBackgroundTask">Task type.</typeparam>
        /// <param name="value">The object to process.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Created task id.</returns>
        public virtual Guid Dispatch<TBackgroundTask>(object value)
        where TBackgroundTask : BackgroundTask
        {
            // The task id is declared outside of the delegate so we 
            // can return it to the caller immediately after dispatching.
            var taskId = Guid.NewGuid();

            // Declare a delegate which handles logging and exception.
            // This delegate will be run in the thread pool.
            async Task WrapperDelegate()
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));

                try
                {
                    using var scope = _serviceScopeFactory.CreateScope();

                    // Note: Do not place any code here that might require an app context to be
                    //       resolved. If we resolve the app context before specifying the factory
                    //       implementation to be used, we get a race condition. The default factory
                    //       will be used in stead of the BackgroundWorkAppContextFactory.

                    // Note: There is no guarantee that an enqueued item will be executed within
                    //       the context that enqueued it. This means that sometimes the app context
                    //       will be created by the asp factory, and sometimes by the background host
                    //       factory. This is a race condition.
                    //       To solve this, we explicitly set the requested implementation type of
                    //       the app context factory to the one used for background tasks.
                    scope.SetAppContextFactoryImplementation<BackgroundWorkAppContextFactory>();

                    // FUTURE: Have the factory perform these assignments. ActivatorUtilities.CreateInstance(params[] ...)?
                    // Assign all required properties to the background task context 
                    // so we can access them from anywhere within the current scope.
                    var appContext = scope.ServiceProvider.GetRequiredService<AppContext>();
                    var backgroundTaskContext = appContext as BackgroundTaskContext;
                    backgroundTaskContext.CancellationToken = cts.Token;
                    backgroundTaskContext.TaskId = taskId;
                    backgroundTaskContext.Value = value;

                    _logger.LogTrace($"Starting background task {backgroundTaskContext.TaskId}");

                    await scope.ServiceProvider.GetService<TBackgroundTask>().ExecuteAsync(backgroundTaskContext);

                    _logger.LogTrace($"Finished background task {backgroundTaskContext.TaskId}");
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Exception in task {taskId}");
                }
            };

            Task.Run(() => WrapperDelegate());

            return taskId;
        }
    }
}
