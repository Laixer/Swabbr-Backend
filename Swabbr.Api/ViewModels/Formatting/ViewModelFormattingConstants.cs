using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Swabbr.Api.ViewModels.Formatting
{

    /// <summary>
    /// Contains formatting constants for our viewmodels like regex expressions.
    /// TODO Use these
    /// </summary>
    public static class ViewModelFormattingConstants
    {

        /// <summary>
        /// Format for dd-mm-yyyy.
        /// </summary>
        public static string RegexDate = @"^\d{2}\-\d{2}\-\d{4}$";

        /// <summary>
        /// Regex for UTC+xx:xx or UTC-xx:xx.
        /// </summary>
        public static string RegexTimeZone = @"^UTC(\+|\-)\d{2}:\d{2}$";

    }

}
