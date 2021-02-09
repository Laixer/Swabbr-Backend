using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Swabbr.Core.BackgroundTasks;
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
        /// <typeparam name="TDefaultAppContextFactory"><see cref="ScopedAppContextFactory{TDefaultAppContextFactory}"/>.</typeparam>
        /// <returns>Service container with swabbr core services.</returns>
        public static IServiceCollection AddSwabbrCoreServices<TDefaultAppContextFactory>(this IServiceCollection services)
            where TDefaultAppContextFactory : class, IAppContextFactory
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            // Make the app context injectable. This uses the scoped app context factory,
            // which has the ability to use a specified app context factory during its
            // scope for app context creation. See ScopedAppContextFactory for details.
            services.AddScoped<IScopedAppContextFactory, ScopedAppContextFactory<TDefaultAppContextFactory>>();
            services.AddScoped((services) =>
            {
                var scopedFactory = services.GetRequiredService<IScopedAppContextFactory>();
                return scopedFactory.CreateAppContext();
            });

            // Configure DI for services
            services.AddScoped<IFollowRequestService, FollowRequestService>();
            services.AddScoped<IUserSelectionService, UserSelectionService>();
            services.AddScoped<IReactionService, ReactionService>();
            services.AddScoped<IVlogService, VlogService>();
            services.AddScoped<IVlogLikeService, VlogLikeService>();
            services.AddScoped<IVlogRequestService, VlogRequestService>();
            services.AddScoped<IUserService, UserService>();

            // Configure DI for background work
            services.AddSingleton<DispatchManager>();
            services.AddBackgroundTask<PostReactionBackgroundTask>();
            services.AddBackgroundTask<PostVlogBackgroundTask>();
            services.AddAppContextFactory<BackgroundWorkAppContextFactory>();

            return services;
        }

        /// <summary>
        ///     Adds an <see cref="IAppContextFactory"/> implementation to the
        ///     services collection.
        /// </summary>
        /// <typeparam name="TAppContextFactory">Implementation type.</typeparam>
        /// <param name="services">Service collection.</param>
        /// <returns><paramref name="services"/> after the operation.</returns>
        public static IServiceCollection AddAppContextFactory<TAppContextFactory>(this IServiceCollection services)
            where TAppContextFactory : class, IAppContextFactory
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            //services.AddScoped(typeof(AppContext), _ => new AppContext());
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IAppContextFactory, TAppContextFactory>());

            return services;
        }
    }
}
