using System;

namespace Swabbr.Infrastructure.Extensions
{
    //TODO Duplicate code with <see cref="Database.TimeZoneInfoHandler"/>.
    /// <summary>
    ///     Contains extension functionality for <see cref="TimeZoneInfo"/>.
    /// </summary>
    public static class TimeZoneInfoExtensions
    {
        /// <summary>
        ///     Formats a time zone info to database format.
        /// </summary>
        /// <param name="self">The object itself.</param>
        /// <returns>Formatted timezone string.</returns>
        public static string ToDatabaseFormat(this TimeZoneInfo self)
        {
            if (self == null)
            {
                throw new ArgumentNullException(nameof(self));
            }

            var offset = self.BaseUtcOffset;
            var hours = (offset.Hours >= 10) ? $"{offset.Hours}" : $"0{offset.Hours}";
            var minutes = (offset.Minutes >= 10) ? $"{offset.Minutes}" : $"0{offset.Minutes}";

            return $"UTC+{hours}:{minutes}";
        }
    }
}
