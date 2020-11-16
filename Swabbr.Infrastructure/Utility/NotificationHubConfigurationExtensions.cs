using Swabbr.Core.Exceptions;
using Swabbr.Core.Extensions;
using Swabbr.Infrastructure.Configuration;
using System;

namespace Swabbr.Infrastructure.Extensions
{
    /// <summary>
    ///     Contains extension functionality for <see cref="NotificationHubConfiguration"/>.
    /// </summary>
    public static class NotificationHubConfigurationExtensions
    {
        /// <summary>
        ///     Validates our notification hub configuration.
        /// </summary>
        /// <param name="config">The object to validate.</param>
        public static void ThrowIfInvalid(this NotificationHubConfiguration config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            if (config.ConnectionString.IsNullOrEmpty()) { throw new ConfigurationException("ANH Connection string is not specified"); }
            if (config.HubName.IsNullOrEmpty()) { throw new ConfigurationException("ANH hub name is not specified"); }
        }
    }
}
