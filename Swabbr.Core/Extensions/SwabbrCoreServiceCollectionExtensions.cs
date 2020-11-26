using Microsoft.Extensions.DependencyInjection;
using Swabbr.Core.Interfaces.Factories;
using Swabbr.Core.Interfaces.Services;
using Swabbr.Core.Services;
using System;

namespace Swabbr.Core.Extensions
{
    /// <summary>
    ///     Adds the swabbr services to the service collection.
    /// </summary>
    public static class SwabbrCoreServiceCollectionExtensions
    {
        /// <summary>
        ///     Adds the swabbr core services to the service container.
        /// </summary>
        /// <remarks>
        ///     This also makes the <see cref="AppContext"/> injectable.
        /// </remarks>
        /// <param name="services">Service container.</param>
        /// <returns>Service container with swabbr core services.</returns>
        public static IServiceCollection AddSwabbrCoreServices(this IServiceCollection services)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            // Make appcontext injectable.
            // TODO QUESTION We might want to pick a single method of implementing this,
            //               Either with the factory, with the context itself or with
            //               explicit assignment as done in Swabbr.Infrastructure.Extensions.SwabbrInfrastructureServiceCollectionExtensions
            services.AddScoped((services) =>
            {
                var factory = services.GetRequiredService<IAppContextFactory>();
                return factory.CreateAppContext();
            });

            // Configure DI for services
            services.AddTransient<IFollowRequestService, FollowRequestService>();
            services.AddTransient<IUserSelectionService, UserSelectionService>();
            services.AddTransient<IHealthCheckService, HealthCheckService>();
            services.AddTransient<IReactionService, ReactionService>();
            services.AddTransient<IVlogService, VlogService>();
            services.AddTransient<IVlogRequestService, VlogRequestService>();
            services.AddTransient<IUserService, UserService>();

            return services;
        }
    }
}
