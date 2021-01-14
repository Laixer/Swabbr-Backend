using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swabbr.Core.BackgroundWork;
using Swabbr.Core.Clients;
using Swabbr.Core.Extensions;
using Swabbr.Core.Interfaces.Clients;
using Swabbr.Core.Interfaces.Factories;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Interfaces.Services;
using Swabbr.Core.Notifications;
using Swabbr.Core.Notifications.BackgroundTasks;
using Swabbr.Core.Notifications.Data;
using Swabbr.Core.Services;
using Swabbr.Core.Storage;
using Swabbr.Infrastructure.Abstractions;
using Swabbr.Infrastructure.Configuration;
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
        ///     Adds the Swabbr Infrastructure services to the service collection.
        /// </summary>
        /// <remarks>
        ///     This adds a null notification service. This can be overwritten by calling
        ///     <see cref="AddSwabbrAnhNotificationInfrastructure(IServiceCollection, string)"/>.
        /// </remarks>
        /// <param name="services">The service collection.</param>
        /// <param name="blobStorageConfigurationSectionName">Name of the blob storage configuration section.</param>
        /// <returns>Same as <paramref name="services"/>.</returns>
        public static IServiceCollection AddSwabbrInfrastructureServices(this IServiceCollection services, 
            string dbConnectionStringName,
            string blobStorageConfigurationSectionName)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            // TODO Seems incorrect
            using var serviceProviderScope = services.BuildServiceProvider().CreateScope();
            var configuration = serviceProviderScope.ServiceProvider.GetRequiredService<IConfiguration>();

            // Add postgresql database functionality
            services.AddSingleton<DatabaseProvider, NpgsqlDatabaseProvider>();
            services.Configure<DatabaseProviderOptions>(options =>
            {
                options.ConnectionStringName = dbConnectionStringName;
            });

            // Add notification package using a null service.
            services.AddScoped<INotificationService, NullNotificationService>();
            services.AddSingleton<INotificationClient, NullNotificationClient>();

            services.AddBackgroundTask<NotifyBackgroundTask<DataVlogRecordRequest>>();
            services.AddBackgroundTask<NotifyBackgroundTask<DataVlogGainedLike>>();
            services.AddBackgroundTask<NotifyBackgroundTask<DataVlogNewReaction>>();
            services.AddBackgroundTask<NotifyBackgroundTask<DataVlogRecordRequest>>();
            services.AddBackgroundTask<NotifyBackgroundTask<DataFollowedProfileVlogPosted>>();

            // Add repositories
            services.AddContextRepository<IFollowRequestRepository, FollowRequestRepository>();
            services.AddContextRepository<IHealthCheckRepository, HealthCheckRepository>();
            services.AddContextRepository<INotificationRegistrationRepository, NotificationRegistrationRepository>();
            services.AddContextRepository<IReactionRepository, ReactionRepository>();
            services.AddContextRepository<IUserRepository, UserRepository>();
            services.AddContextRepository<IVlogLikeRepository, VlogLikeRepository>();
            services.AddContextRepository<IVlogRepository, VlogRepository>();

            // Add storage package
            services.AddSingleton<IBlobStorageService, SpacesBlobStorageService>();
            services.Configure<BlobStorageOptions>(configuration.GetSection(blobStorageConfigurationSectionName));

            return services;
        }

        /// <summary>
        ///     Adds the Swabbr notification services to the service collection using an
        ///     Azure Notification Hub implementation.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         At the moment this uses the Azure Notification Hub and thus requires
        ///         a <see cref="Configuration.AzureNotificationHubConfiguration"/> section
        ///         in the configuration.
        ///     </para>
        ///     <para>
        ///         This replaces all existing notification services, clients and factories.
        ///     </para>
        /// </remarks>
        /// <param name="services">The service collection.</param>
        /// <param name="configurationSectionName">Name of the azure notification hub configuration section.</param>
        /// <returns>Same as <paramref name="services"/>.</returns>
        public static IServiceCollection AddSwabbrAnhNotificationInfrastructure(this IServiceCollection services, 
            string configurationSectionName)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            using var serviceProviderScope = services.BuildServiceProvider().CreateScope();
            var configuration = serviceProviderScope.ServiceProvider.GetRequiredService<IConfiguration>();

            // Add Azure Notification Hub notification package
            services.AddOrReplace<INotificationService, NotificationService>(ServiceLifetime.Scoped);
            services.AddOrReplace<INotificationClient, AnhNotificationClient>(ServiceLifetime.Singleton);
            services.AddOrReplace<INotificationFactory, NotificationFactory>(ServiceLifetime.Singleton);
            services.Configure<AzureNotificationHubConfiguration>(configuration.GetSection(configurationSectionName));

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

                // TODO Move outside of the DI container.
                databaseContextBase.DatabaseProvider = serviceProvider.GetService<DatabaseProvider>();

                // Note: We don't resolve for the AppContext factory or scoped factory
                //       here to prevent multiple AppContext instances from being created.
                //       The AppContext should be instantiated once during each scope.
                databaseContextBase.AppContext = serviceProvider.GetRequiredService<Core.AppContext>();

                return repository;
            });

            return services;
        }
    }
}
