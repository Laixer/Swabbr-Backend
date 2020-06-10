using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Swabbr.Api.Utility
{
    /// <summary>
    /// Contains formatting constants for our regex expressions.
    /// </summary>
    public static class RegexConstants
    {
        /// <summary>
        /// Format for dd-mm-yyyy.
        /// </summary>
        public const string RegexDate = @"^\d{2}\-\d{2}\-\d{4}$";

        /// <summary>
        /// Regex for UTC+xx:xx or UTC-xx:xx.
        /// </summary>
        public const string RegexTimeZone = @"^UTC(\+|\-)\d{2}:\d{2}$";
    }
}
