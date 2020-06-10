using Laixer.Infra.Npgsql;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swabbr.AzureMediaServices.Clients;
using Swabbr.AzureMediaServices.Configuration;
using Swabbr.AzureMediaServices.Interfaces.Clients;
using Swabbr.AzureMediaServices.Interfaces.Services;
using Swabbr.AzureMediaServices.Services;
using Swabbr.Core.Configuration;
using Swabbr.Core.Interfaces.Clients;
using Swabbr.Core.Interfaces.Notifications;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Interfaces.Services;
using Swabbr.Core.Notifications;
using Swabbr.Core.Services;
using Swabbr.Infrastructure.Configuration;
using Swabbr.Infrastructure.Database;
using Swabbr.Infrastructure.Notifications;
using Swabbr.Infrastructure.Notifications.JsonExtraction;
using Swabbr.Infrastructure.Repositories;
using System;

[assembly: FunctionsStartup(typeof(Swabbr.AzureFunctions.Startup))]
namespace Swabbr.AzureFunctions
{
    /// <summary>
    /// Sets up our DI.
    /// </summary>
    public class Startup : FunctionsStartup
    {
        /// <summary>
        /// Parameterless constructor is required by Azure Functions.
        /// </summary>
        public Startup() { }

        /// <summary>
        /// Sets up our DI.
        /// </summary>
        /// <param name="builder"><see cref="IFunctionsHostBuilder"/></param>
        public override void Configure(IFunctionsHostBuilder builder)
        {
            if (builder == null) { throw new ArgumentNullException(nameof(builder)); }

            // Add configuration
            builder.Services.AddOptions<SwabbrConfiguration>().Configure<IConfiguration>((settings, configuration) =>
            {
                configuration.GetSection("SwabbrConfiguration").Bind(settings);
            });
            builder.Services.AddOptions<NotificationHubConfiguration>().Configure<IConfiguration>((settings, configuration) =>
            {
                configuration.GetSection("NotificationHub").Bind(settings);
            });
            builder.Services.AddOptions<AMSConfiguration>().Configure<IConfiguration>((settings, configuration) =>
            {
                configuration.GetSection("AzureMediaServices").Bind(settings);
            });
            builder.Services.AddOptions<LogicAppsConfiguration>().Configure<IConfiguration>((settings, configuration) =>
            {
                configuration.GetSection("LogicAppsConfiguration").Bind(settings);
            });

            // Add postgresql database functionality
            NpgsqlSetup.Setup();
            builder.Services.AddTransient<IDatabaseProvider, NpgsqlDatabaseProvider>();
            builder.Services.AddOptions<NpgsqlDatabaseProviderOptions>().Configure<IConfiguration>((settings, configuration) =>
                configuration.GetSection("DatabaseInternal").Bind(settings));

            // Configure DI for data repositories
            builder.Services.AddTransient<ILivestreamRepository, LivestreamRepository>();
            builder.Services.AddTransient<INotificationRegistrationRepository, NotificationRegistrationRepository>();
            builder.Services.AddTransient<IReactionRepository, ReactionRepository>();
            builder.Services.AddTransient<IUserRepository, UserRepository>();
            builder.Services.AddTransient<IVlogRepository, VlogRepository>();
            builder.Services.AddTransient<IVlogLikeRepository, VlogLikeRepository>();

            // Configure DI for services
            builder.Services.AddTransient<IAMSTokenService, AMSTokenService>();
            builder.Services.AddTransient<IHashDistributionService, HashDistributionService>();
            builder.Services.AddTransient<ILivestreamPoolService, AMSLivestreamPoolService>();
            builder.Services.AddTransient<ILivestreamService, AMSLivestreamService>();
            builder.Services.AddTransient<IPlaybackService, AMSPlaybackService>();
            builder.Services.AddTransient<INotificationService, NotificationService>();
            builder.Services.AddTransient<IReactionService, AMSReactionService>();
            builder.Services.AddTransient<IStorageService, AMSStorageService>();
            builder.Services.AddTransient<IUserStreamingHandlingService, UserStreamingHandlingService>();
            builder.Services.AddTransient<IUserService, UserService>();
            builder.Services.AddTransient<IVlogService, VlogService>();
            builder.Services.AddTransient<IVlogTriggerService, VlogTriggerService>();

            // Configure DI for client services
            builder.Services.AddTransient<INotificationClient, NotificationClient>();
            builder.Services.AddTransient<INotificationBuilder, NotificationBuilder>();
            builder.Services.AddTransient<INotificationJsonExtractor, NotificationJsonExtractor>();
            builder.Services.AddTransient<IAMSClient, AMSClient>();
        }
    }
}
