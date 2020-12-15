using Swabbr.Core;
using System;
using System.Data.Common;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Swabbr.Infrastructure.Extensions
{
    /// <summary>
    ///     DbDataReader extensions.
    /// </summary>
    internal static class DbDataReaderExtensions
    {
        /// <summary>
        ///     Return value as integer.
        /// </summary>
        /// <param name="reader">Input reader to extend.</param>
        /// <param name="ordinal">Column ordinal.</param>
        /// <returns>Value as integer.</returns>
        public static int GetInt(this DbDataReader reader, int ordinal)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            return reader.GetInt32(ordinal);
        }

        /// <summary>
        ///     Return value as unsigned integer.
        /// </summary>
        /// <param name="reader">Input reader to extend.</param>
        /// <param name="ordinal">Column ordinal.</param>
        /// <returns>Value as integer.</returns>
        public static uint GetUInt(this DbDataReader reader, int ordinal)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            return (uint)reader.GetInt(ordinal);
        }

        /// <summary>
        ///     Return value as nullable integer.
        /// </summary>
        /// <param name="reader">Input reader to extend.</param>
        /// <param name="ordinal">Column ordinal.</param>
        /// <returns>Value as nullable integer.</returns>
        public static int? GetSafeInt(this DbDataReader reader, int ordinal)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            if (reader.IsDBNull(ordinal))
            {
                return null;
            }

            return reader.GetInt(ordinal);
        }

        /// <summary>
        ///     Return value as nullable unsigned integer.
        /// </summary>
        /// <param name="reader">Input reader to extend.</param>
        /// <param name="ordinal">Column ordinal.</param>
        /// <returns>Value as nullable unsigned integer.</returns>
        public static uint? GetSafeUInt(this DbDataReader reader, int ordinal)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            if (reader.IsDBNull(ordinal))
            {
                return null;
            }

            return reader.GetUInt(ordinal);
        }

        /// <summary>
        ///     Return value as nullable double.
        /// </summary>
        /// <param name="reader">Input reader to extend.</param>
        /// <param name="ordinal">Column ordinal.</param>
        /// <returns>Value as nullable double.</returns>
        public static double? GetSafeDouble(this DbDataReader reader, int ordinal)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            if (reader.IsDBNull(ordinal))
            {
                return null;
            }

            return reader.GetDouble(ordinal);
        }

        /// <summary>
        ///     Return value as nullable float.
        /// </summary>
        /// <param name="reader">Input reader to extend.</param>
        /// <param name="ordinal">Column ordinal.</param>
        /// <returns>Value as nullable float.</returns>
        public static float? GetSafeFloat(this DbDataReader reader, int ordinal)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            if (reader.IsDBNull(ordinal))
            {
                return null;
            }

            return reader.GetFloat(ordinal);
        }

        /// <summary>
        ///     Return value as nullable string.
        /// </summary>
        /// <param name="reader">Input reader to extend.</param>
        /// <param name="ordinal">Column ordinal.</param>
        /// <returns>Value as nullable string.</returns>
        public static string GetSafeString(this DbDataReader reader, int ordinal)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
        }

        /// <summary>
        ///     Return value as nullable datetime.
        /// </summary>
        /// <param name="reader">Input reader to extend.</param>
        /// <param name="ordinal">Column ordinal.</param>
        /// <returns>Datetime or null.</returns>
        public static DateTime? GetSafeDateTime(this DbDataReader reader, int ordinal)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            return reader.IsDBNull(ordinal) ? null : (DateTime?)reader.GetDateTime(ordinal);
        }

        /// <summary>
        ///     Return value as nullable boolean.
        /// </summary>
        /// <param name="reader">Input reader to extend.</param>
        /// <param name="ordinal">Column ordinal.</param>
        /// <returns>Boolean or null.</returns>
        public static bool? GetSafeBoolean(this DbDataReader reader, int ordinal)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            return reader.IsDBNull(ordinal) ? null : (bool?)reader.GetBoolean(ordinal);
        }

        /// <summary>
        ///     Return value as nullable decimal.
        /// </summary>
        /// <param name="reader">Input reader to extend.</param>
        /// <param name="ordinal">Column ordinal.</param>
        /// <returns>decimal or null.</returns>
        public static decimal? GetSafeDecimal(this DbDataReader reader, int ordinal)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            return reader.IsDBNull(ordinal) ? null : (decimal?)reader.GetDecimal(ordinal);
        }

        /// <summary>
        ///     Return value as nullable <typeparamref name="TFieldType"/>.
        /// </summary>
        /// <typeparam name="TFieldType">Type to return value to.</typeparam>
        /// <param name="reader">Input reader to extend.</param>
        /// <param name="ordinal">Column ordinal.</param>
        /// <returns>Value or null.</returns>
        public static TFieldType GetSafeFieldValue<TFieldType>(this DbDataReader reader, int ordinal)
            where TFieldType : class
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            return reader.IsDBNull(ordinal) ? null : reader.GetFieldValue<TFieldType>(ordinal);
        }

        /// <summary>
        ///     Return value as <typeparamref name="TimeZoneInfo"/>.
        /// </summary>
        /// <remarks>
        ///     This uses our custom timezone storage format, being
        ///     <see cref="RegexConstants.TimeZone"/>.
        /// </remarks>
        /// <typeparam name="TFieldType">Type to return value to.</typeparam>
        /// <param name="reader">Input reader to extend.</param>
        /// <param name="ordinal">Column ordinal.</param>
        /// <returns>Value or null.</returns>
        public static TimeZoneInfo GetTimeZoneInfo(this DbDataReader reader, int ordinal)
        {
            if (reader is null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            var asString = reader.GetString(ordinal);

            // Perform regex matching to check the expected pattern.
            if (!Regex.IsMatch(asString, RegexConstants.TimeZone)) 
            { 
                throw new FormatException();
            }

            bool isPlus = asString[3] == '+';

            var hour = int.Parse(asString.Substring(4, 2), CultureInfo.InvariantCulture);
            var minute = int.Parse(asString.Substring(7, 2), CultureInfo.InvariantCulture);

            var timeSpan = new TimeSpan(hours: isPlus ? hour : -hour, minutes: minute, seconds: 0);

            return TimeZoneInfo.CreateCustomTimeZone(asString, timeSpan, asString, asString);
        }
    }
}
