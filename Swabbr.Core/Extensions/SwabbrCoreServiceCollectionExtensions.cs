using Microsoft.Extensions.DependencyInjection;
using Swabbr.Core.BackgroundWork;
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

            // Make the AppContext itself injectable.
            services.AddScoped((services) =>
            {
                var factory = services.GetRequiredService<IAppContextFactory>();
                return factory.CreateAppContext();
            });

            // Configure DI for services
            services.AddScoped<IFollowRequestService, FollowRequestService>();
            services.AddScoped<IUserSelectionService, UserSelectionService>();
            services.AddScoped<IReactionService, ReactionService>();
            services.AddScoped<IVlogService, VlogService>();
            services.AddScoped<IVlogRequestService, VlogRequestService>();
            services.AddScoped<IUserService, UserService>();

            // Configure DI for background work
            services.AddSingleton<DispatchManager>();

            return services;
        }
    }
}
