using Laixer.Infra.Npgsql;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Swabbr.AzureMediaServices.Clients;
using Swabbr.AzureMediaServices.Configuration;
using Swabbr.AzureMediaServices.Extensions;
using Swabbr.AzureMediaServices.Interfaces.Clients;
using Swabbr.AzureMediaServices.Services;
using Swabbr.Core.Configuration;
using Swabbr.Core.Factories;
using Swabbr.Core.Interfaces.Clients;
using Swabbr.Core.Interfaces.Factories;
using Swabbr.Core.Interfaces.Notifications;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Interfaces.Services;
using Swabbr.Core.Notifications;
using Swabbr.Core.Services;
using Swabbr.Core.Types;
using Swabbr.Core.Utility;
using Swabbr.Infrastructure.Configuration;
using Swabbr.Infrastructure.Database;
using Swabbr.Infrastructure.Notifications;
using Swabbr.Infrastructure.Notifications.JsonExtraction;
using Swabbr.Infrastructure.Repositories;
using Swabbr.Infrastructure.Utility;
using Swabbr.WowzaStreamingCloud.Configuration;
using System;
using System.Configuration;

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
            // TODO Fix the config mess later
            // Add Azure App Configuration remote
            //var connectionString = builder.Services.BuildServiceProvider().GetRequiredService<IConfiguration>().GetValue<string>("AzureAppConfig:ConnectionString");
            //if (string.IsNullOrEmpty(connectionString)) { throw new ConfigurationException("Missing connection string for AzureAppConfig"); }
            //var myBuilder = new ConfigurationBuilder();
            //myBuilder.AddAzureAppConfiguration(connectionString);
            //var newConfiguration = myBuilder.Build();
            //// builder.AddConfigurationClient
            //// Add configurations
            //builder.Services.Configure<SwabbrConfiguration>(options =>
            //{
            //    newConfiguration.GetSection("SwabbrConfiguration").Bind(options);
            //});
            //builder.Services.Configure<NotificationHubConfiguration>(options =>
            //{
            //    newConfiguration.GetSection("NotificationHub").Bind(options);
            //});
            //builder.Services.Configure<AMSConfiguration>(options =>
            //{
            //    newConfiguration.GetSection("AzureMediaServices").Bind(options);
            //});

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

            // Check configuration
            var servicesBuilt = builder.Services.BuildServiceProvider();
            servicesBuilt.GetRequiredService<IOptions<SwabbrConfiguration>>().Value.ThrowIfInvalid();
            servicesBuilt.GetRequiredService<IOptions<NotificationHubConfiguration>>().Value.ThrowIfInvalid();
            servicesBuilt.GetRequiredService<IOptions<AMSConfiguration>>().Value.ThrowIfInvalid();
            servicesBuilt.GetRequiredService<IOptions<LogicAppsConfiguration>>().Value.ThrowIfInvalid();

            // Add postgresql database functionality
            NpgsqlSetup.Setup();
            builder.Services.AddTransient<IDatabaseProvider, NpgsqlDatabaseProvider>();
            builder.Services.AddOptions<NpgsqlDatabaseProviderOptions>().Configure<IConfiguration>((settings, configuration) =>
                configuration.GetSection("DatabaseInternal").Bind(settings));

            // Configure DI for data repositories
            builder.Services.AddTransient<ILivestreamRepository, LivestreamRepository>();
            builder.Services.AddTransient<INotificationRegistrationRepository, NotificationRegistrationRepository>();
            builder.Services.AddTransient<IReactionRepository, ReactionRepository>();
            builder.Services.AddTransient<IRequestRepository, RequestRepository>();
            builder.Services.AddTransient<IUserRepository, UserRepository>();
            builder.Services.AddTransient<IVlogRepository, VlogRepository>();
            builder.Services.AddTransient<IVlogLikeRepository, VlogLikeRepository>();

            // Configure DI for services
            builder.Services.AddTransient<IHashDistributionService, HashDebugEvenDistributionService>(); // TODO FIX
            builder.Services.AddTransient<ILivestreamPoolService, AMSLivestreamPoolService>();
            builder.Services.AddTransient<ILivestreamService, AMSLivestreamService>();
            builder.Services.AddTransient<ILivestreamPlaybackService, AMSLivestreamPlaybackService>();
            builder.Services.AddTransient<INotificationService, NotificationService>();
            builder.Services.AddTransient<IReactionService, ReactionService>();
            builder.Services.AddTransient<IReactionUploadService, ReactionUploadService>();
            builder.Services.AddTransient<IStorageService, AMSStorageService>();
            builder.Services.AddTransient<IUserStreamingHandlingService, UserStreamingHandlingService>();
            builder.Services.AddTransient<ITranscodingService, AMSTranscodingService>();
            builder.Services.AddTransient<IUserService, UserService>();
            builder.Services.AddTransient<IVlogService, VlogService>();
            builder.Services.AddTransient<IVlogTriggerService, VlogTriggerService>();

            // Configure DI for client services
            builder.Services.AddTransient<INotificationClient, NotificationClient>();
            builder.Services.AddTransient<INotificationBuilder, NotificationBuilder>();
            builder.Services.AddTransient<INotificationJsonExtractor, NotificationJsonExtractor>();
            builder.Services.AddTransient<IAMSClient, AMSClient>();
            builder.Services.AddSingleton<IHttpClientFactory, HttpClientFactory>();
        }

    }

}
