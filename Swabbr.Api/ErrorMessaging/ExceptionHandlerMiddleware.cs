using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Interfaces;
using System;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace Swabbr.Api.ErrorMessaging
{
    /// <summary>
    ///     Exception handler middleware for a specified exception type.
    /// </summary>
    /// <remarks>
    ///     This redirects to <see cref="ExceptionHandlerOptions.ErrorControllerPath"/>
    ///     for the error handling response.
    /// </remarks>
    /// <typeparam name="TException">The exception type to handle.</typeparam>
    public class ExceptionHandlerMiddleware<TException>
        where TException : Exception
    {
        private readonly RequestDelegate _next;
        private readonly IExceptionMapper<TException> _mapper;
        private readonly ExceptionHandlerOptions _options;
        private readonly ILogger<ExceptionHandlerMiddleware<TException>> _logger;

        /// <summary>
        ///     Create new instance.
        /// </summary>
        public ExceptionHandlerMiddleware(RequestDelegate next, 
            IExceptionMapper<TException> mapper, 
            IOptions<ExceptionHandlerOptions> options, 
            ILogger<ExceptionHandlerMiddleware<TException>> logger)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        ///     Invoke this piece of middleware.
        /// </summary>
        /// <param name="context">Http request context.</param>
        public virtual Task InvokeAsync(HttpContext context)
        {
            ExceptionDispatchInfo edi;

            try
            {
                var task = _next(context);
                if (task.IsCompletedSuccessfully)
                {
                    return Task.CompletedTask;
                }
                else if (task.IsCanceled)
                {
                    // The task was cancelled by either the user or the backend itself.
                    edi = ExceptionDispatchInfo.Capture(new OperationAbortedException());
                }
                else if (task.IsFaulted)
                {
                    // The task had an unhandled exception.
                    edi = ExceptionDispatchInfo.Capture(task.Exception);
                }
                else
                {
                    // The task hasn't been executed yet.
                    return ProcessAsync(context, task);
                }
            }
            catch (TException e)
            {
                edi = ExceptionDispatchInfo.Capture(e);
            }

            // If we reach this, some exception has been thrown.
            // Continue execution outside of the try/catch block.
            return HandleExceptionAsync(edi, context);
        }

        /// <summary>
        ///     Process a task asynchronously.
        /// </summary>
        /// <param name="context">The http context.</param>
        /// <param name="task">The task to process.</param>
        private async Task ProcessAsync(HttpContext context, Task task)
        {
            ExceptionDispatchInfo edi;

            try
            {
                await task;
                return;
            }
            catch (TException e)
            {
                edi = ExceptionDispatchInfo.Capture(e);
            }

            // If we reach this, some exception has been thrown.
            // Continue execution outside of the try/catch block.
            await HandleExceptionAsync(edi, context);
        }

        /// <summary>
        ///     Handle a thrown exception.
        /// </summary>
        /// <param name="edi">The thrown exception info.</param>
        /// <param name="context">The http context.</param>
        protected virtual async Task HandleExceptionAsync(ExceptionDispatchInfo edi, HttpContext context)
        {
            // We're only interested in the root cause of the exception.
            // Unpack the base exception in case of an aggregate exception.
            if (edi.SourceException is AggregateException)
            {
                edi = ExceptionDispatchInfo.Capture((edi.SourceException as AggregateException).GetBaseException());
            }

            // If the exception is not of type TException, throw it upwards.
            if (edi.SourceException is not TException)
            {
                edi.Throw();
            }

            // We can't do anything if the response has already started, just abort.
            // This means headers have already been sent to the client.
            if (context.Response.HasStarted)
            {
                _logger.LogError(edi.SourceException, "Could not do anything, response has already started");

                edi.Throw();
            }

            // Save original context path for later restoration in case we can't handle the exception.
            var originalPath = context.Request.Path;

            try
            {
                // Prepare the context for being passed back up the chain.
                ClearHttpContext(context);

                // Add a generated error message to the http context.
                context.Features.Set(_mapper.Map(edi.SourceException as TException));

                // Pass back up the chain to reach the error handling controller.
                context.Request.Path = _options.ErrorControllerPath ?? throw new InvalidOperationException("No error handler path specified");
                await _next(context);

                return;
            }
            catch (Exception e)
            {
                // Only log, we re-throw later.
                _logger.LogError(e, "Could not handle exception, re-throwing original");
            }
            finally
            {
                // Set the context path to what it was.
                context.Request.Path = originalPath;
            }

            // If we reach this we couldn't handle the exception.
            // Re-throw it upwards.
            edi.Throw();
        }

        /// <summary>
        ///     Prepares the http context for re-usage.
        /// </summary>
        /// <param name="context">Http context.</param>
        private void ClearHttpContext(HttpContext context)
        {
            _logger.LogDebug("Clear response");

            // Clear any response properties which have been set.
            context.Response.Clear();

            // An endpoint may have already been set. Since we're going to re-
            // invoke the middleware pipeline we need to reset the endpoint and
            // route values to ensure things are re-calculated.
            context.SetEndpoint(endpoint: null);
            context.Features.Get<IRouteValuesFeature>()?.RouteValues?.Clear();
        }
    }
}
