﻿using Dapper;
using System;
using System.Data;
using System.Text.RegularExpressions;

namespace Swabbr.Infrastructure.Database
{

    /// <summary>
    /// Tells our sql mapper how to handle <see cref="TimeZoneInfo"/> objects.
    /// </summary>
    public sealed class TimeZoneInfoHandler : SqlMapper.TypeHandler<TimeZoneInfo>
    {

        /// <summary>
        /// TODO This might be worth double checking.
        /// TODO Centralize regex maybe
        /// </summary>
        /// <param name="value">Value to be parsed</param>
        /// <returns><see cref="TimeZoneInfo"/></returns>
        public override TimeZoneInfo Parse(object value)
        {
            if (value == null) { throw new ArgumentNullException(nameof(value)); }
            var asString = value as string;
            if (asString == null) { throw new InvalidCastException(nameof(value)); }

            // REGEX check
            if (!Regex.IsMatch(asString, @"^UTC(\+|-)\d\d:\d\d$")) { throw new FormatException(nameof(value)); }

            bool isPlus = asString[3] == '+';

            var hour = int.Parse(asString.Substring(4, 2));
            var minute = int.Parse(asString.Substring(7, 2));

            var timeSpan = new TimeSpan(hours: isPlus ? hour : -hour, minutes: minute, seconds: 0);
            return TimeZoneInfo.CreateCustomTimeZone(asString, timeSpan, asString, asString);
        }

        public override void SetValue(IDbDataParameter parameter, TimeZoneInfo value)
        {
            if (parameter == null) { throw new ArgumentNullException(nameof(parameter)); }
            if (value == null) { throw new ArgumentNullException(nameof(value)); }

            var offset = value.BaseUtcOffset;
            var hours = (offset.Hours >= 10) ? offset.Hours.ToString() : $"0{offset.Hours.ToString()}";
            var minutes = (offset.Minutes >= 10) ? offset.Minutes.ToString() : $"0{offset.Minutes.ToString()}";
            parameter.Value = $"UTC+{hours}:{minutes}";
        }

    }

}