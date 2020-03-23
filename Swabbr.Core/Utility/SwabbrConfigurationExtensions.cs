using Laixer.Utility.Exceptions;
using Swabbr.Core.Types;

namespace Swabbr.Core.Utility
{
    public static class SwabbrConfigurationExtensions
    {

        /// <summary>
        /// Validates a <see cref="SwabbrConfiguration"/>.
        /// </summary>
        /// <param name="config"><see cref="SwabbrConfiguration"/></param>
        public static void ThrowIfInvalid(this SwabbrConfiguration config)
        {
            if (config == null) { throw new ConfigurationException(nameof(config)); }

            // TODO uint instead?
            if (config.DailyVlogRequestLimit < 0) { throw new ConfigurationRangeException(nameof(config.DailyVlogRequestLimit)); }
            if (config.VlogLengthMaxSeconds < 0) { throw new ConfigurationRangeException(nameof(config.VlogLengthMaxSeconds)); }
            if (config.VlogLengthMinSeconds < 0) { throw new ConfigurationRangeException(nameof(config.VlogLengthMinSeconds)); }
            if (config.VlogRequestTimeoutSeconds < 0) { throw new ConfigurationRangeException(nameof(config.VlogRequestTimeoutSeconds)); }
        }

    }

}
