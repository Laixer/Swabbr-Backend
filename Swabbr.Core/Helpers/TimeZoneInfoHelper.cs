using System;

namespace Swabbr.Core.Helpers
{
    /// <summary>
    ///     Contains utility functionality for formatting 
    ///     our timezone info.
    /// </summary>
    public static class TimeZoneInfoHelper
    { 
        /// <summary>
        ///     Converts a timezone object to the expected
        ///     database format timezone.
        /// </summary>
        /// <remarks>
        ///     If <paramref name="timeZoneInfo"/> is null, 
        ///     this returns null.
        /// </remarks>
        /// <param name="timeZoneInfo">The timezone object.</param>
        /// <returns>Formatted string.</returns>
        public static string MapTimeZoneToStringOrNull(TimeZoneInfo timeZoneInfo)
        {
            if (timeZoneInfo is null)
            {
                return null;
            }

            var offset = timeZoneInfo.BaseUtcOffset;

            var hours = (offset.Hours >= 10) ? $"{offset.Hours}" : $"0{offset.Hours}";
            var minutes = (offset.Minutes >= 10) ? $"{offset.Minutes}" : $"0{offset.Minutes}";

            return $"UTC+{hours}:{minutes}";
        }
    }
}
