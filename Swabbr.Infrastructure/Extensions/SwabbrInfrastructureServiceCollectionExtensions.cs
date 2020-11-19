using Microsoft.Extensions.DependencyInjection;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Interfaces.Services;
using Swabbr.Infrastructure.Database;
using Swabbr.Infrastructure.Notifications;
using Swabbr.Infrastructure.Providers;
using Swabbr.Infrastructure.Repositories;
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
            // TODO This is entirely Dapper specific. This will be removed in the future.
            NpgsqlSetup.Setup();
            services.AddTransient<IDatabaseProvider, NpgsqlDatabaseProvider>();
            services.Configure<NpgsqlDatabaseProviderOptions>(options =>
            {
                options.ConnectionStringName = dbConnectionStringName;
            });

            // Add repositories
            services.AddTransient<IFollowRequestRepository, FollowRequestRepository>();
            services.AddTransient<IHealthCheckRepository, HealthCheckRepository>();
            services.AddTransient<ILivestreamRepository, LivestreamRepository>();
            services.AddTransient<INotificationRegistrationRepository, NotificationRegistrationRepository>();
            services.AddTransient<IReactionRepository, ReactionRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IUserWithStatsRepository, UserWithStatsRepository>();
            services.AddTransient<IVlogLikeRepository, VlogLikeRepository>();
            services.AddTransient<IVlogRepository, VlogRepository>();

            // Add notification package
            services.AddTransient<INotificationService, NotificationService>();
            services.AddSingleton<NotificationClient>();

            return services;
        }
    }
}
