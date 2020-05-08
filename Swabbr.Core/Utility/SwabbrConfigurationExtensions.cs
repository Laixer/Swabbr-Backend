using Laixer.Utility.Exceptions;
using Swabbr.Core.Types;
using System;

namespace Swabbr.Core.Utility
{
    public static class SwabbrConfigurationExtensions
    {

        /// <summary>
        /// Validates a <see cref="SwabbrConfiguration"/>.
        /// TODO These values are defaulted to 0, which doesn't always trigger an exception.
        /// </summary>
        /// <param name="config"><see cref="SwabbrConfiguration"/></param>
        public static void ThrowIfInvalid(this SwabbrConfiguration config)
        {
            if (config == null) { throw new ConfigurationException(nameof(config)); }

            if (config.DailyVlogRequestLimit < 0) { throw new ConfigurationRangeException(nameof(config.DailyVlogRequestLimit)); }
            if (config.VlogLengthMaxSeconds < 0) { throw new ConfigurationRangeException(nameof(config.VlogLengthMaxSeconds)); }
            if (config.VlogLengthMinSeconds < 0) { throw new ConfigurationRangeException(nameof(config.VlogLengthMinSeconds)); }
            if (config.VlogRequestTimeoutMinutes < 0) { throw new ConfigurationRangeException(nameof(config.VlogRequestTimeoutMinutes)); }
            if (config.VlogRequestStartTimeMinutes < 0) { throw new ConfigurationRangeException(nameof(config.VlogRequestStartTimeMinutes)); }
            if (config.VlogRequestStartTimeMinutes >= TimeSpan.FromHours(24).TotalMinutes) { throw new ConfigurationRangeException(nameof(config.VlogRequestStartTimeMinutes)); }
            if (config.VlogRequestEndTimeMinutes < 0) { throw new ConfigurationRangeException(nameof(config.VlogRequestEndTimeMinutes)); }
            if (config.VlogRequestEndTimeMinutes >= TimeSpan.FromHours(24).TotalMinutes) { throw new ConfigurationRangeException(nameof(config.VlogRequestEndTimeMinutes)); }
            if (config.VlogRequestStartTimeMinutes >= config.VlogRequestEndTimeMinutes) { throw new ConfigurationException("Start time must be < end time"); }
        }

    }

}
