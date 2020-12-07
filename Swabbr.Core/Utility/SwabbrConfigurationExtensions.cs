using Swabbr.Core.Configuration;
using Swabbr.Core.Exceptions;
using System;

namespace Swabbr.Core.Utility
{
    /// <summary>
    ///     Contains extension functionality for <see cref="SwabbrConfiguration"/>.
    /// </summary>
    public static class SwabbrConfigurationExtensions
    {
        /// <summary>
        ///     Validates a <see cref="SwabbrConfiguration"/>.
        /// </summary>
        /// <param name="config">The configuration to check.</param>
        public static void ThrowIfInvalid(this SwabbrConfiguration config)
        {
            if (config is null)
            {
                throw new ConfigurationException(nameof(config));
            }

            if (config.ReactionLengthMaxInSeconds <= 0) { throw new ConfigurationRangeException(nameof(config.ReactionLengthMaxInSeconds)); }
            if (config.MaxDailyVlogRequestLimit < 0) { throw new ConfigurationRangeException(nameof(config.MaxDailyVlogRequestLimit)); }
            if (config.VlogLengthMaxSeconds <= 0) { throw new ConfigurationRangeException(nameof(config.VlogLengthMaxSeconds)); }
            if (config.VlogLengthMinSeconds < 0) { throw new ConfigurationRangeException(nameof(config.VlogLengthMinSeconds)); }
            if (config.VlogRequestTimeoutMinutes <= 0) { throw new ConfigurationRangeException(nameof(config.VlogRequestTimeoutMinutes)); }
            if (config.VlogRequestStartTimeMinutes <= 0) { throw new ConfigurationRangeException(nameof(config.VlogRequestStartTimeMinutes)); }
            if (config.VlogRequestStartTimeMinutes >= TimeSpan.FromHours(24).TotalMinutes) { throw new ConfigurationRangeException(nameof(config.VlogRequestStartTimeMinutes)); }
            if (config.VlogRequestEndTimeMinutes <= 0) { throw new ConfigurationRangeException(nameof(config.VlogRequestEndTimeMinutes)); }
            if (config.VlogRequestEndTimeMinutes >= TimeSpan.FromHours(24).TotalMinutes) { throw new ConfigurationRangeException(nameof(config.VlogRequestEndTimeMinutes)); }
            if (config.VlogRequestStartTimeMinutes >= config.VlogRequestEndTimeMinutes) { throw new ConfigurationException("Start time must be < end time"); }
        }
    }
}
