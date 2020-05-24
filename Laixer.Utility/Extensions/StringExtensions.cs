using System;

namespace Laixer.Utility.Extensions
{

    /// <summary>
    /// Contains extensions for the <see cref="string"/> class / type.
    /// </summary>
    public static class StringExtensions
    {

        /// <summary>
        /// Throws a <see cref="ArgumentNullException"/> if a string is null or empty.
        /// </summary>
        /// <param name="s"><see cref="string"/></param>
        public static void ThrowIfNullOrEmpty(this string s)
        {
            if (s == null) { throw new ArgumentNullException(nameof(s)); }
            if (string.IsNullOrEmpty(s)) { throw new ArgumentNullException("String is null or empty"); }
        }

        public static bool IsNullOrEmpty(this string s)
        {
            return string.IsNullOrEmpty(s);
        }


    }
}
