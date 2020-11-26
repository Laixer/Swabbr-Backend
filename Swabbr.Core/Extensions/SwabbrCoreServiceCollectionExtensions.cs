using Microsoft.Extensions.DependencyInjection;
using Swabbr.Core.Interfaces.Factories;
using Swabbr.Core.Interfaces.Services;
using Swabbr.Core.Services;

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
            // Make appcontext injectable.
            services.AddScoped((services) =>
            {
                var factory = services.GetRequiredService<IAppContextFactory>();
                return factory.CreateAppContext();
            });

            // TODO Move this to core extension
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
