using Microsoft.Extensions.DependencyInjection;
using Swabbr.AzureMediaServices.Clients;
using Swabbr.AzureMediaServices.Services;
using Swabbr.Core.Interfaces.Services;
using System;

namespace Swabbr.AzureMediaServices.Extensions
{
    /// <summary>
    ///     Extends the service collection for the Swabbr 
    ///     Azure Media Services package.
    /// </summary>
    public static class SwabbrAmsServiceCollectionExtensions
    {
        /// <summary>
        ///     Adds the Swabbr Infrastructure services to the
        ///     service collection.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <returns>Same as <paramref name="services"/>.</returns>
        public static IServiceCollection AddSwabbrAmsServices(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            // TODO Options

            // Add services.
            services.AddTransient<ILivestreamPoolService, AMSLivestreamPoolService>();
            services.AddTransient<ILivestreamService, AMSLivestreamService>();
            services.AddTransient<IPlaybackService, AMSPlaybackService>();
            services.AddTransient<IReactionService, AMSReactionService>();
            services.AddTransient<IReactionWithThumbnailService, AMSReactionWithThumbnailService>();
            services.AddTransient<IStorageService, AMSStorageService>();

            // Add clients and helpers.
            services.AddSingleton<AMSClient>();
            services.AddSingleton<AMSTokenService>();

            return services;
        }
    }
}
