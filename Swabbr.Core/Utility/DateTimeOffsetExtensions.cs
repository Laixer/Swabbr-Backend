﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Swabbr.Core.Utility
{
    
    /// <summary>
    /// Contains extension functionality for <see cref="DateTimeOffset"/>.
    /// </summary>
    public static class DateTimeOffsetExtensions
    {

        public static int GetMinutes(this DateTimeOffset self)
        {
            if (self == null) { throw new ArgumentNullException(nameof(self)); }
            return (60 * self.Hour) + self.Minute;
        }

        public static DateTimeOffset ToTriggerMinute(this DateTimeOffset self)
        {
            if (self == null) { throw new ArgumentNullException(nameof(self)); }
            return new DateTimeOffset(self.Year, self.Month, self.Day, self.Hour, self.Minute, 0, self.Offset);
        }

        /// <summary>
        /// TODO Implement
        /// </summary>
        /// <param name="self">This <see cref="DateTimeOffset"/></param>
        public static void ThrowIfNullOrEmpty(this DateTimeOffset self)
        {
            if (self == null) { throw new ArgumentNullException(nameof(self)); }
            if (self.IsNullOrEmpty()) { throw new ArgumentNullException("DateTimeOffset struct is null or empty"); }
        }

        /// <summary>
        /// TODO Implement.
        /// </summary>
        /// <param name="self">This <see cref="DateTimeOffset"/></param>
        /// <returns><see cref="bool"/></returns>
        public static bool IsNullOrEmpty(this DateTimeOffset self)
        {
            if (self == null) { throw new ArgumentNullException(nameof(self)); }
            return false;
        }

    }

}
