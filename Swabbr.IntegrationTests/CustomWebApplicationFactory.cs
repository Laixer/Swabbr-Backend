using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swabbr.Core.Extensions;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Interfaces.Services;
using Swabbr.Core.Services;
using Swabbr.Testing.Repositories;

namespace Swabbr.IntegrationTests
{
    /// <summary>
    ///     Base class to create a testable ASP web application.
    /// </summary>
    /// <typeparam name="TStartup">Original startup class.</typeparam>
    public abstract class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup>
        where TStartup : class
    {
        /// <summary>
        ///     Create new instance.
        /// </summary>
        public CustomWebApplicationFactory()
            => ClientOptions.HandleCookies = false;

        /// <summary>
        ///     This can be used to configure specific non-testing services.
        /// </summary>
        /// <param name="services">Service collection.</param>
        protected virtual void ConfigureServices(IServiceCollection services)
        {
        }

        /// <summary>
        ///     Configures all testing repositories and null services.
        /// </summary>
        /// <param name="services">Service collection.</param>
        protected virtual void ConfigureTestServices(IServiceCollection services)
        {
            // Add repositories
            services.AddOrReplace<IFollowRequestRepository, TestFollowRequestRepository>(ServiceLifetime.Scoped);
            services.AddOrReplace<INotificationRegistrationRepository, TestNotificationRegistrationRepository>(ServiceLifetime.Scoped);
            services.AddOrReplace<IReactionRepository, TestReactionRepository>(ServiceLifetime.Scoped);
            services.AddOrReplace<IUserRepository, TestUserRepository>(ServiceLifetime.Scoped);
            services.AddOrReplace<IVlogLikeRepository, TestVlogLikeRepository>(ServiceLifetime.Scoped);
            services.AddOrReplace<IVlogRepository, TestVlogRepository>(ServiceLifetime.Scoped);

            // Add null services
            services.AddOrReplace<IBlobStorageService, NullBlobStorageService>(ServiceLifetime.Singleton);
            services.AddOrReplace<INotificationService, NullNotificationService>(ServiceLifetime.Scoped);

            services.AddLogging((logging) =>
            {
                logging.SetMinimumLevel(LogLevel.Trace);
                logging.AddConsole();
            });
        }

        /// <summary>
        ///     Configures the web host services.
        /// </summary>
        /// <remarks>
        ///     First calls <see cref="ConfigureServices"/>,
        ///     then calls <see cref="ConfigureTestServices"/>.
        /// </remarks>
        /// <param name="builder">Web host builder.</param>
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(ConfigureServices);
            builder.ConfigureTestServices(ConfigureTestServices);
        }
    }
}
