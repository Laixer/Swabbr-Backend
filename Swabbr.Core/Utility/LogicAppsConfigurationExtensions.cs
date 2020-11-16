using Swabbr.Core.Configuration;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Extensions;
using System;

namespace Swabbr.Core.Utility
{

    /// <summary>
    /// Contains extension functionality for <see cref="LogicAppsConfiguration"/>.
    /// </summary>
    public static class LogicAppsConfigurationExtensions
    {

        /// <summary>
        /// Throws if invalid.
        /// </summary>
        /// <param name="config"><see cref="LogicAppsConfiguration"/></param>
        public static void ThrowIfInvalid(this LogicAppsConfiguration config)
        {
            if (config == null) { throw new ArgumentNullException(nameof(config)); }

            if (config.EndpointUserConnectTimeout.IsNullOrEmpty()) { throw new ConfigurationException(nameof(config.EndpointUserConnectTimeout)); }
        }

    }

}
