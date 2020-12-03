using Microsoft.Extensions.DependencyInjection;
using Swabbr.Core.Interfaces.Factories;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Interfaces.Services;
using Swabbr.Infrastructure.Abstractions;
using Swabbr.Infrastructure.Notifications;
using Swabbr.Infrastructure.Providers;
using Swabbr.Infrastructure.Repositories;
using Swabbr.Infrastructure.Storage;
using System;

namespace Swabbr.Infrastructure.Extensions
{
    /// <summary>
    ///     Extends the service collection for the Swabbr 
    ///     infrastructure package.
    /// </summary>
    public static class SwabbrInfrastructureServiceCollectionExtensions
    {
        /// <summary>
        ///     Adds the Swabbr Infrastructure services to the
        ///     service collection.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <returns>Same as <paramref name="services"/>.</returns>
        public static IServiceCollection AddSwabbrInfrastructureServices(this IServiceCollection services, string dbConnectionStringName)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            // Add postgresql database functionality
            services.AddSingleton<DatabaseProvider, NpgsqlDatabaseProvider>();
            services.Configure<DatabaseProviderOptions>(options =>
            {
                options.ConnectionStringName = dbConnectionStringName;
            });

            // Add repositories
            services.AddContextRepository<IFollowRequestRepository, FollowRequestRepository>();
            services.AddContextRepository<IHealthCheckRepository, HealthCheckRepository>();
            services.AddContextRepository<INotificationRegistrationRepository, NotificationRegistrationRepository>();
            services.AddContextRepository<IReactionRepository, ReactionRepository>();
            services.AddContextRepository<IUserRepository, UserRepository>();
            services.AddContextRepository<IVlogLikeRepository, VlogLikeRepository>();
            services.AddContextRepository<IVlogRepository, VlogRepository>();

            // Add notification package
            services.AddTransient<INotificationService, NotificationService>();
            services.AddSingleton<NotificationClient>();

            // Add storage package
            services.AddSingleton<IBlobStorageService, SpacesBlobStorageService>();

            return services;
        }

        /// <summary>
        ///     Adds a repository which depends on our application context
        ///     to the service container. 
        /// </summary>
        /// <remarks>
        ///     The <typeparamref name="TImplementation"/> has to extend
        ///     <see cref="DatabaseContextBase"/>.
        /// </remarks>
        /// <typeparam name="TService">Repository interface.</typeparam>
        /// <typeparam name="TImplementation">Repository implementation.</typeparam>
        /// <param name="services">The service collection.</param>
        /// <returns>Same as <paramref name="services"/> so we can chain calls.</returns>
        private static IServiceCollection AddContextRepository<TService, TImplementation>(this IServiceCollection services)
            where TService : class
            where TImplementation : DatabaseContextBase, TService, new()
        {
            services.AddScoped<TService, TImplementation>(serviceProvider =>
            {
                var repository = new TImplementation();
                var databaseContextBase = repository as DatabaseContextBase;

                var appContextFactory = serviceProvider.GetRequiredService<IAppContextFactory>();

                databaseContextBase.DatabaseProvider = serviceProvider.GetService<DatabaseProvider>();
                databaseContextBase.AppContext = appContextFactory.CreateAppContext();

                return repository;
            });

            return services;
        }
    }
}
