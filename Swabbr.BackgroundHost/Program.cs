using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Swabbr.BackgroundHost.BackgroundTasks;
using Swabbr.BackgroundHost.Services;
using Swabbr.Core;
using Swabbr.Core.BackgroundWork;
using Swabbr.Core.Extensions;
using Swabbr.Infrastructure.Extensions;

namespace Swabbr.BackgroundHost
{
    /// <summary>
    ///     Application entry class.
    /// </summary>
    public class Program
    {
        /// <summary>
        ///     Application entry point.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        public static void Main(string[] args) 
            => CreateHostBuilder(args).Build().Run();

        /// <summary>
        ///     Creates our host builder.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        /// <returns>Host builder.</returns>
        public static IHostBuilder CreateHostBuilder(string[] args) 
            => Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    // Bind main configuration properties.
                    services.Configure<SwabbrConfiguration>(hostContext.Configuration.GetSection("Swabbr"));

                    // Add swabbr services.
                    // Note: this will always use the background work app context factory.
                    services.AddSwabbrCoreServices<BackgroundWorkAppContextFactory>();
                    services.AddSwabbrInfrastructureServices("DatabaseInternal", "BlobStorage");
                    services.AddSwabbrAnhNotificationInfrastructure("AzureNotificationHub");

                    // Add hosted services
                    services.AddHostedService<VlogRequestPeriodicHostedService>();

                    // Add background tasks
                    services.AddBackgroundTask<VlogRequestCycleBackgroundTask>();
                });
    }
}
