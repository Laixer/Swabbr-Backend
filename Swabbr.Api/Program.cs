using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Extensions;
using System.Threading.Tasks;

namespace Swabbr
{
    /// <summary>
    ///     Entry class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        ///     Application entry point.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        public static Task Main(string[] args)
            => CreateHostBuilder(args).Build().RunAsync();

        /// <summary>
        ///     Creates our host builder.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        /// <returns>Host builder instance.</returns>
        public static IHostBuilder CreateHostBuilder(string[] args)
            => Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var settings = config.Build();
                    var connectionString = settings["ConnectionStrings:AzureAppConfig"];
                    if (string.IsNullOrEmpty(connectionString)) { throw new ConfigurationException("Missing connection string for AzureAppConfig"); }
                    config.AddAzureAppConfiguration(options =>
                    {
                        options.Connect(connectionString);
                    });
                });
    }
}
