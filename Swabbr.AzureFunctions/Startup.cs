using Laixer.Infra.Npgsql;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swabbr.AzureMediaServices.Configuration;
using Swabbr.AzureMediaServices.Services;
using Swabbr.Core.Interfaces.Clients;
using Swabbr.Core.Interfaces.Notifications;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Interfaces.Services;
using Swabbr.Core.Services;
using Swabbr.Core.Types;
using Swabbr.Infrastructure.Configuration;
using Swabbr.Infrastructure.Database;
using Swabbr.Infrastructure.Notifications;
using Swabbr.Infrastructure.Notifications.JsonExtraction;
using Swabbr.Infrastructure.Repositories;
using Swabbr.WowzaStreamingCloud.Configuration;
using Swabbr.WowzaStreamingCloud.Services;
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
        /// Sets up our DI.
        /// </summary>
        /// <param name="builder"><see cref="IFunctionsHostBuilder"/></param>
        public override void Configure(IFunctionsHostBuilder builder)
        {
            // Add configurations
            // TODO Connection string issues (even though it works now)
            builder.Services.AddOptions<SwabbrConfiguration>().Configure<IConfiguration>((settings, configuration) =>
            {
                configuration.GetSection("SwabbrConfiguration").Bind(settings);
            });
            builder.Services.AddOptions<NotificationHubConfiguration>().Configure<IConfiguration>((settings, configuration) =>
            {
                configuration.GetSection("NotificationHub").Bind(settings);
                //settings.ConnectionString = ConfigurationManager.ConnectionStrings["AzureNotificationHub"].ConnectionString;
            });
            builder.Services.AddOptions<WowzaStreamingCloudConfiguration>().Configure<IConfiguration>((settings, configuration) =>
            {
                configuration.GetSection("WowzaStreamingCloud").Bind(settings);
            });
            builder.Services.AddOptions<AMSConfiguration>().Configure<IConfiguration>((settings, configuration) =>
            {
                configuration.GetSection("AzureMediaServices").Bind(settings);
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
            builder.Services.AddTransient<IRequestRepository, RequestRepository>();
            builder.Services.AddTransient<IUserRepository, UserRepository>();
            builder.Services.AddTransient<IVlogRepository, VlogRepository>();

            // Configure DI for services
            builder.Services.AddTransient<IHashDistributionService, HashDebugDistributionService>(); // TODO FIX
            builder.Services.AddTransient<ILivestreamPoolService, WowzaLivestreamPoolService>();
            builder.Services.AddTransient<ILivestreamService, WowzaLivestreamService>();
            builder.Services.AddTransient<INotificationService, NotificationService>();
            builder.Services.AddTransient<IReactionService, ReactionService>();
            builder.Services.AddTransient<IReactionUploadService, ReactionUploadService>();
            builder.Services.AddTransient<IStorageService, AMSStorageService>();
            builder.Services.AddTransient<ITranscodingService, AMSTranscodingService>();
            builder.Services.AddTransient<IUserService, UserService>();
            builder.Services.AddTransient<IVlogTriggerService, VlogTriggerService>();

            // Configure DI for client services
            builder.Services.AddTransient<INotificationClient, NotificationClient>();
            builder.Services.AddTransient<INotificationJsonExtractor, NotificationJsonExtractor>();
        }

    }

}
