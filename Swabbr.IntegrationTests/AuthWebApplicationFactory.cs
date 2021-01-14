using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Swabbr.IntegrationTests.Authentication;
using System.Linq;

namespace Swabbr.IntegrationTests
{
    /// <summary>
    ///     Creates a web application with authentication enabled.
    /// </summary>
    /// <typeparam name="TStartup">Original startup class.</typeparam>
    public class AuthWebApplicationFactory<TStartup> : CustomWebApplicationFactory<TStartup>
        where TStartup : class
    {
        /// <summary>
        ///     Calls <see cref="ConfigureTestServices"/> on the base and then
        ///     adds our test authentication handler.
        /// </summary>
        /// <param name="services">Service collection.</param>
        protected override void ConfigureTestServices(IServiceCollection services)
        {
            // First call base to ensure test services are setup correctly.
            base.ConfigureTestServices(services);

            // Add the test authentication options to the services.
            services.AddSingleton<TestAuthenticationSchemeOptions>();

            // Add the test authentication handler to the services.
            services.AddAuthentication(TestAuthHandler.AuthenticationScheme)
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.AuthenticationScheme, options => { });

            var x = services.Where(x => x.ImplementationType == typeof(TestAuthHandler));
            if (x.Any())
            {

            }
        }
    }
}
