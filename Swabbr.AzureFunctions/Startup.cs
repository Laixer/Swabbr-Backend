using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swabbr.AzureMediaServices.Configuration;
using Swabbr.AzureMediaServices.Extensions;
using Swabbr.Core.Configuration;
using Swabbr.Core.Interfaces.Services;
using Swabbr.Core.Services;
using Swabbr.Infrastructure.Configuration;
using Swabbr.Infrastructure.Extensions;
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

            // Add Azure Media Services to DI
            builder.Services.AddSwabbrAmsServices();

            // Configure DI for services
            builder.Services.AddTransient<IHashDistributionService, HashDistributionService>();
            builder.Services.AddTransient<IUserStreamingHandlingService, UserStreamingHandlingService>();
            builder.Services.AddTransient<IUserService, UserService>();
            builder.Services.AddTransient<IVlogService, VlogService>();
            builder.Services.AddTransient<IVlogTriggerService, VlogTriggerService>();
        }
    }
}
