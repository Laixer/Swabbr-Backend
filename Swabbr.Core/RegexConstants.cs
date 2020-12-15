namespace Swabbr.Core
{
    /// <summary>
    ///     Contains application specific regex constants.
    /// </summary>
    public static class RegexConstants
    {
        /// <summary>
        ///     Regex representing our timezone data format:
        ///     UTC+xx:xx or UTC-xx:xx
        /// </summary>
        public const string TimeZone = @"^UTC(\+|-)\d\d:\d\d$";
    }
}
