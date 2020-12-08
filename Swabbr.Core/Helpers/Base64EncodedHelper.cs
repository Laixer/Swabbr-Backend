using System;

namespace Swabbr.Core.Helpers
{
    /// <summary>
    ///     Contains helping functions to check base 64 encoded content.
    /// </summary>
    public static class Base64EncodedHelper
    {
        /// <summary>
        ///     Checks if some string is base 64 encoded.
        /// </summary>
        /// <remarks>
        ///     This returns false if the string is empty.
        /// </remarks>
        /// <param name="input">The string to check.</param>
        public static bool IsBase64Encoded(string input)
        {
            if (input is null)
            {
                throw new ArgumentNullException(nameof(input));
            }
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }

            var buffer = new Span<byte>(new byte[input.Length]);
            return Convert.TryFromBase64String(input, buffer, out int _);
        }
    }
}
