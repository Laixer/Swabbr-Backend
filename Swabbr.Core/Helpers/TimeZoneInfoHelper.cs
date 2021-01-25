using System;
using System.Globalization;

namespace Swabbr.Core.Helpers
{
    // TODO Making custom time zone info objects has a limit according to some old doc I read, look into this!
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
            var absHours = Math.Abs(offset.Hours);

            var hours = (absHours >= 10) 
                ? $"{absHours}" 
                : $"0{absHours}";
            var minutes = (offset.Minutes >= 10) 
                ? $"{offset.Minutes}" 
                : $"0{offset.Minutes}";

            return (offset.Hours >= 0)
                ? $"+{hours}:{minutes}"
                : $"-{hours}:{minutes}";
        }

        /// <summary>
        ///     Map a formatted timezone string according to ISO 8601 to a
        ///     <see cref="TimeZoneInfo"/> object.
        /// </summary>
        /// <remarks>
        ///     The expected format is +xx:xx or -xx:xx which is relative to 
        ///     UTC. If <paramref name="s"/> is null or empty, null is returned.
        /// </remarks>
        /// <param name="s">Formatted string.</param>
        /// <returns>Mapped <see cref="TimeZoneInfo"/> or null.</returns>
        public static TimeZoneInfo MapStringToTimeZone(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return null;
            }

            // Perform regex matching to check the expected pattern.
            if (!RegexConstants.TimeZoneRegex.IsMatch(s))
            {
                throw new FormatException();
            }

            bool isPlus = s[0] == '+';

            var hour = int.Parse(s.Substring(1, 2), CultureInfo.InvariantCulture);
            var minute = int.Parse(s.Substring(4, 2), CultureInfo.InvariantCulture);

            var timeSpan = new TimeSpan(hours: isPlus ? hour : -hour, minutes: minute, seconds: 0);

            return TimeZoneInfo.CreateCustomTimeZone(s, timeSpan, s, s);
        }
    }
}
