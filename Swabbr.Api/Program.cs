using Laixer.Utility.Exceptions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace Swabbr
{
    public static class Program
    {
        public static Task Main(string[] args)
            => CreateHostBuilder(args).Build().RunAsync();

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