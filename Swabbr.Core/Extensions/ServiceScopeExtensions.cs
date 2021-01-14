using Microsoft.Extensions.DependencyInjection;
using Swabbr.Core.Interfaces.Factories;
using System;

namespace Swabbr.Core.Extensions
{
    /// <summary>
    ///     Extension functionality for <see cref="IServiceScope"/>.
    /// </summary>
    public static class ServiceScopeExtensions
    {
        /// <summary>
        ///     Configures our scope to use a specified implementation of
        ///     <see cref="IAppContextFactory"/> for the rest of the scope.
        /// </summary>
        /// <typeparam name="TAppContextFactory">Implementation type.</typeparam>
        /// <param name="scope">Service scope.</param>
        /// <returns><paramref name="scope"/> for chaining calls.</returns>
        public static IServiceScope SetAppContextFactoryImplementation<TAppContextFactory>(this IServiceScope scope)
            where TAppContextFactory : class, IAppContextFactory
        {
            if (scope is null)
            {
                throw new ArgumentNullException(nameof(scope));
            }

            var scopedAppContextFactory = scope.ServiceProvider.GetRequiredService<IScopedAppContextFactory>();
            scopedAppContextFactory.SetAppContextFactoryType<TAppContextFactory>();

            return scope;
        }
    }
}
