using Laixer.Utility.Extensions;
using System;
using System.Text.RegularExpressions;

namespace Swabbr.Api.Parsing
{

    /// <summary>
    /// Parses user input to <see cref="TimeZoneInfo"/>.
    /// </summary>
    internal static class TimeZoneInfoParser
    {

        private static string pattern => @"^UTC(\+|-)\d\d:\d\d$";

        internal static TimeZoneInfo Parse(string userInput)
        {
            userInput.ThrowIfNullOrEmpty();
            if (!Regex.IsMatch(userInput, pattern)) { throw new FormatException(nameof(userInput)); }

            bool isPlus = userInput[3] == '+';

            var hour = int.Parse(userInput.Substring(4, 2));
            var minute = int.Parse(userInput.Substring(7, 2));

            var timeSpan = new TimeSpan(hours: isPlus ? hour : -hour, minutes: minute, seconds: 0);
            return TimeZoneInfo.CreateCustomTimeZone(userInput, timeSpan, userInput, userInput);
        }

    }

}
