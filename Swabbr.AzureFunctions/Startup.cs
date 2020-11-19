using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swabbr.AzureMediaServices.Clients;
using Swabbr.AzureMediaServices.Configuration;
using Swabbr.AzureMediaServices.Interfaces.Clients;
using Swabbr.AzureMediaServices.Interfaces.Services;
using Swabbr.AzureMediaServices.Services;
using Swabbr.Core.Configuration;
using Swabbr.Infrastructure.Extensions;
using Swabbr.Core.Interfaces.Services;
using Swabbr.Core.Services;
using Swabbr.Infrastructure.Configuration;
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

            // Configure Swabbr infrastructure
            builder.Services.AddSwabbrInfrastructureServices("DatabaseInternal");

            // Configure DI for services
            builder.Services.AddTransient<IAMSTokenService, AMSTokenService>();
            builder.Services.AddTransient<IHashDistributionService, HashDistributionService>();
            builder.Services.AddTransient<ILivestreamPoolService, AMSLivestreamPoolService>();
            builder.Services.AddTransient<ILivestreamService, AMSLivestreamService>();
            builder.Services.AddTransient<IPlaybackService, AMSPlaybackService>();
            builder.Services.AddTransient<IReactionService, AMSReactionService>();
            builder.Services.AddTransient<IStorageService, AMSStorageService>();
            builder.Services.AddTransient<IUserStreamingHandlingService, UserStreamingHandlingService>();
            builder.Services.AddTransient<IUserService, UserService>();
            builder.Services.AddTransient<IVlogService, VlogService>();
            builder.Services.AddTransient<IVlogTriggerService, VlogTriggerService>();

            // Configure DI for client services
            builder.Services.AddTransient<IAMSClient, AMSClient>();
        }
    }
}
