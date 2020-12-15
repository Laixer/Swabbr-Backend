using Microsoft.Extensions.DependencyInjection;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Interfaces;

namespace Swabbr.Api.ErrorMessaging
{
    /// <summary>
    ///     Contains service collection extension functionality
    ///     for adding custom exception handlers.
    /// </summary>
    public static class ExceptionHandlerServiceCollectionExtensions
    {
        /// <summary>
        ///     Add the swabbr exception mapper to the services.
        /// </summary>
        /// <param name="services">Service collection.</param>
        /// <returns>Service collection with swabbr exception mapper.</returns>
        public static IServiceCollection AddSwabbrExceptionMapper(this IServiceCollection services)
            => services.AddSingleton<IExceptionMapper<SwabbrCoreException>, SwabbrAspExceptionMapper>();
    }
}
