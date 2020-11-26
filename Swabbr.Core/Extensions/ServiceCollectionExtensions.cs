using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Linq;

namespace Swabbr.Core.Extensions
{
    /// <summary>
    ///     Contains extension functionality for <see cref="IServiceCollection"/>.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        ///     Replaces all services in <see cref="IServiceCollection"/> with the same service type as descriptor
        ///     and adds descriptor to the collection. If the service is not found then a new descriptor is added
        ///     to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">Service collection.</param>
        /// <param name="lifetime">Desired service lifetime if none exist in the service collection.</param>
        /// <returns>Service collection after modification.</returns>
        public static IServiceCollection AddOrReplace<TService, TImplementation>(this IServiceCollection services, ServiceLifetime lifetime)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            var serviceList = services.Where(s => s.ServiceType == typeof(TService));
            if (serviceList != null && serviceList.Any())
            {
                // NOTE: The ToList() generates a complete list with known size. 
                //       This circumvents in-place modification of the enumerable.
                foreach (var service in serviceList.ToList())
                {
                    services.Replace(new ServiceDescriptor(typeof(TService), typeof(TImplementation), service.Lifetime));
                }
            }
            else
            {
                services.Add(new ServiceDescriptor(typeof(TService), typeof(TImplementation), lifetime));
            }

            return services;
        }
    }
}
