using System;

namespace Swabbr.Infrastructure.Utility
{

    /// <summary>
    /// Contains utility functions for SQL.
    /// </summary>
    internal static class SqlUtility
    {

        /// <summary>
        /// Formats a <see cref="DateTimeOffset"/> to SQL format.
        /// </summary>
        /// <param name="date"><see cref="DateTimeOffset"/></param>
        /// <returns>SQL formatted string</returns>
        internal static string FormatDateTime(DateTimeOffset date)
        {
            if (date == null) { throw new ArgumentNullException(nameof(date)); }
            return date.ToString("yyyy-MM-dd HH:mm:ss"); // TODO Check this
        }

    }

}
