using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Swabbr.Core.BackgroundWork
{
    /// <summary>
    ///     Contains extension functionality to add background
    ///     tasks to the service collection.
    /// </summary>
    public static class BackgroundTasksServiceCollectionExtensions
    {
        /// <summary>
        ///     Add a background task to the service collection.
        /// </summary>
        /// <typeparam name="TBackgroundTask">Task type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <returns>Service collection with background task added.</returns>
        public static IServiceCollection AddBackgroundTask<TBackgroundTask>(this IServiceCollection services)
            where TBackgroundTask : BackgroundTask
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            // TODO Why add it explicitly as well?
            services.AddTransient(typeof(TBackgroundTask));
            services.TryAddEnumerable(ServiceDescriptor.Transient<BackgroundTask, TBackgroundTask>());

            return services;
        }
    }
}
