using System.Text.RegularExpressions;

namespace Swabbr.Core
{
    // TODO Timeout for regex validation.
    /// <summary>
    ///     Contains application specific regex constants.
    /// </summary>
    public static class RegexConstants
    {
        /// <summary>
        ///     Regex representing our timezone data format
        ///     UTC+xx:xx or UTC-xx:xx
        /// </summary>
        public static readonly Regex TimeZoneRegex = new Regex(@"^(\+|-)\d\d:\d\d$", RegexOptions.Compiled);
    }
}
