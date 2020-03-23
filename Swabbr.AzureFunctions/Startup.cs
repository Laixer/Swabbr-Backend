using Dapper;
using Laixer.Infra.Npgsql;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swabbr.Core.BackgroundWorkers;
using Swabbr.Core.Interfaces.BackgroundWorkers;
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
            builder.Services.AddOptions<SwabbrConfiguration>()
                .Configure<IConfiguration>((settings, configuration) =>
                    configuration.GetSection("SwabbrConfiguration").Bind(settings));
            builder.Services.AddOptions<NotificationHubConfiguration>()
                .Configure<IConfiguration>((settings, configuration) =>
                    configuration.GetSection("NotificationHub").Bind(settings));
            builder.Services.AddOptions<WowzaStreamingCloudConfiguration>()
                .Configure<IConfiguration>((settings, configuration) =>
                    configuration.GetSection("WowzaStreamingCloud").Bind(settings));

            // Add postgresql database functionality
            NpgsqlSetup.Setup();
            SqlMapper.AddTypeHandler(new UriHandler());
            SqlMapper.AddTypeHandler(new FollowRequestStatusHandler()); // TODO Look at this
            builder.Services.AddTransient<IDatabaseProvider, NpgsqlDatabaseProvider>();
            builder.Services.Configure<NpgsqlDatabaseProviderOptions>(options => { options.ConnectionStringName = "DatabaseInternal"; });

            // Configure DI for data repositories
            builder.Services.AddTransient<IUserRepository, UserRepository>();
            //builder.Services.AddTransient<IUserWithStatsRepository, UserWithStatsRepository>();
            //builder.Services.AddTransient<IFollowRequestRepository, FollowRequestRepository>();
            builder.Services.AddTransient<IVlogRepository, VlogRepository>();
            //builder.Services.AddTransient<IVlogLikeRepository, VlogLikeRepository>();
            //builder.Services.AddTransient<IReactionRepository, ReactionRepository>();
            builder.Services.AddTransient<ILivestreamRepository, LivestreamRepository>();
            builder.Services.AddTransient<INotificationRegistrationRepository, NotificationRegistrationRepository>();
            builder.Services.AddTransient<IRequestRepository, RequestRepository>();

            // Configure DI for services
            //builder.Services.AddTransient<IUserService, UserService>();
            //builder.Services.AddTransient<IVlogService, VlogService>();
            builder.Services.AddTransient<IVlogTriggerService, VlogTriggerService>();
            //builder.Services.AddTransient<IReactionService, ReactionService>();
            //builder.Services.AddTransient<IFollowRequestService, FollowRequestService>();
            builder.Services.AddTransient<ILivestreamService, WowzaLivestreamService>();
            builder.Services.AddTransient<ILivestreamPoolService, WowzaLivestreamPoolService>();
            //builder.Services.AddTransient<ILivestreamPlaybackService, WowzaLivestreamPlaybackService>();
            builder.Services.AddTransient<INotificationService, NotificationService>();
            //builder.Services.AddTransient<IUserStreamingHandlingService, UserStreamingHandlingService>();
            //builder.Services.AddTransient<IDeviceRegistrationService, DeviceRegistrationService>();

            // Configure DI for client services
            builder.Services.AddTransient<INotificationClient, NotificationClient>();
            builder.Services.AddTransient<INotificationJsonExtractor, NotificationJsonExtractor>();

            // Add background workers
            builder.Services.AddTransient<IVlogTriggerWorker, VlogTriggerWorker>();

        }

    }

}
