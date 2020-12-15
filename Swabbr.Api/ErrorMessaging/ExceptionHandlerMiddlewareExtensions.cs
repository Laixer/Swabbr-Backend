using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using Swabbr.Core.Exceptions;
using System;

namespace Swabbr.Api.ErrorMessaging
{
    /// <summary>
    ///     Contains middleware extension functionality.
    /// </summary>
    public static class ExceptionHandlerMiddlewareExtensions
    {
        /// <summary>
        ///     Add the swabbr exception handler to the middleware pipeline.
        /// </summary>
        /// <param name="builder">Application builder.</param>
        /// <param name="errorControllerPath">Path to redirect to for error repsonses.</param>
        /// <returns>Application builder with swabbr exception handler.</returns>
        public static IApplicationBuilder UseSwabbrExceptionHandler(this IApplicationBuilder builder, string errorControllerPath)
        {
            // TODO [DisallowNull]
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            var options = new ExceptionHandlerOptions
            {
                ErrorControllerPath = errorControllerPath
            };

            builder.UseMiddleware<ExceptionHandlerMiddleware<SwabbrCoreException>>(Options.Create(options));

            return builder;
        }
    }
}
