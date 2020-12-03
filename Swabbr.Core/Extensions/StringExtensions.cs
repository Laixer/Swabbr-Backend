using System;

namespace Swabbr.Core.Extensions
{
    /// <summary>
    ///     Contains extension functionality for <see cref="string"/>.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        ///     Throws an <see cref="ArgumentNullException"/>
        ///     if a given string is null or empty.
        /// </summary>
        /// <param name="s">The string to check.</param>
        public static void ThrowIfNullOrEmpty(this string s)
        {
            if (s is null || string.IsNullOrEmpty(s)) 
            { 
                throw new ArgumentNullException(nameof(s)); 
            }
        }

        /// <summary>
        ///     Checks if a string is null or empty.
        /// </summary>
        /// <param name="s">The string to check.</param>
        public static bool IsNullOrEmpty(this string s)
            => string.IsNullOrEmpty(s);
    }
}
