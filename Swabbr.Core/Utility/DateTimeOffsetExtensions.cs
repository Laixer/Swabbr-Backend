using System;
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

    }

}
