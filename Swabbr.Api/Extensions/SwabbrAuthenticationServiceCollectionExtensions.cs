using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swabbr.Api.Authentication;
using System;

namespace Swabbr.Api.Extensions
{
    /// <summary>
    ///     Extension functionality for our authentication provider.
    /// </summary>
    public static class SwabbrAuthenticationServiceCollectionExtensions
    {
        /// <summary>
        ///     Adds authentication to the service container.
        /// </summary>
        /// <remarks>
        ///     This expects the <see cref="IConfiguration"/>
        ///     to contain a section by the name of "Jwt" with the 
        ///     desired <see cref="JwtConfiguration"/> values.
        /// </remarks>
        /// <param name="services">Service collection.</param>
        /// <param name="configurationSection">Name of the configuration section.</param>
        /// <returns>Service collection with authentication.</returns>
        public static IServiceCollection AddSwabbrAuthentication(this IServiceCollection services, string configurationSection)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            // TODO Is this correct?
            services.AddSingleton<TokenService>();

            using var serviceProviderScope = services.BuildServiceProvider().CreateScope();
            var configuration = serviceProviderScope.ServiceProvider.GetRequiredService<IConfiguration>();

            services.Configure<JwtConfiguration>(configuration.GetSection(configurationSection));

            return services;
        }
    }
}
