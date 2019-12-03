using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Swabbr
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((ctx, builder) =>
                {
                    var config = builder.Build();
                })
                .UseStartup<Startup>();
    }
}