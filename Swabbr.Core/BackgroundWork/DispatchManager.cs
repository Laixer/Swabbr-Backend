using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Swabbr.Core.BackgroundWork
{
    /// <summary>
    ///     Manager for dispatching background tasks.
    /// </summary>
    /// <remarks>
    ///     This currently only supports asynchronous work.
    /// </remarks>
    public class DispatchManager : IDisposable
    {
        protected readonly IServiceScopeFactory _serviceScopeFactory;
        protected readonly ILogger<DispatchManager> _logger;

        protected readonly CancellationTokenSource _cts = new CancellationTokenSource();

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
        ///         If no cancellation token is specified, one is
        ///         assigned by this class itself. The source of this
        ///         token will be disposed upon disposal of this object,
        ///         before which cancellation will be requested.
        ///     </para>
        /// </remarks>
        /// <typeparam name="TBackgroundTask">Task type.</typeparam>
        /// <param name="value">The object to process.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Created task id.</returns>
        public virtual Guid Dispatch<TBackgroundTask>(object value, CancellationToken? token = null)
            where TBackgroundTask : BackgroundTask
        {
            var context = new BackgroundTaskContext
            {
                CancellationToken = token ?? _cts.Token,
                Value = value
            };

            // Declare a delegate which handles logging and exception.
            // This delegate will be run in the thread pool.
            async Task WrapperDelegate()
            {
                try
                {
                    // TODO How to ensure that the cancellation token is passed on to the AppContext?
                    using var scope = _serviceScopeFactory.CreateScope();
                    var backgroundTask = scope.ServiceProvider.GetRequiredService<TBackgroundTask>();

                    _logger.LogTrace($"Starting background task {context.Id}");

                    await backgroundTask.ExecuteAsync(context);

                    _logger.LogTrace($"Finished background task {context.Id}");
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Exception in task {context.Id}");
                }
            };

            // Note: Passing a cancellation token to Task.Run() allows us to cancel
            //       the work if it has not yet started. This is deliberately skipped.
            //       The work can be cancelled using the context cancellation token.
            Task.Run(() => WrapperDelegate());

            return context.Id;
        }

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
