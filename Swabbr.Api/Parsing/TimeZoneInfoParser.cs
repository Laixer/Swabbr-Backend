using Laixer.Utility.Extensions;
using Swabbr.Api.Utility;
using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Swabbr.Api.Parsing
{
    /// <summary>
    /// Parses user input to <see cref="TimeZoneInfo"/>.
    /// </summary>
    internal static class TimeZoneInfoParser
    {
        internal static TimeZoneInfo Parse(string userInput)
        {
            userInput.ThrowIfNullOrEmpty();
            if (!Regex.IsMatch(userInput, RegexConstants.RegexTimeZone)) { throw new FormatException(nameof(userInput)); }

            bool isPlus = userInput[3] == '+';

            var hour = int.Parse(userInput.Substring(4, 2), CultureInfo.InvariantCulture);
            var minute = int.Parse(userInput.Substring(7, 2), CultureInfo.InvariantCulture);

            var timeSpan = new TimeSpan(hours: isPlus ? hour : -hour, minutes: minute, seconds: 0);
            return TimeZoneInfo.CreateCustomTimeZone(userInput, timeSpan, userInput, userInput);
        }
    }
}
