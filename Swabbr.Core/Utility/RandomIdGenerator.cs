using System;
using System.Text;

namespace Swabbr.Core.Utility
{
    /// <summary>
    /// Generates random ids for us.
    /// </summary>
    public static class RandomIdGenerator
    {
        public const string Alphabet = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

        /// <summary>
        /// Generates a random id based on the <see cref="Alphabet"/>.
        /// </summary>
        /// <remarks>
        /// The maximum <paramref name="length"/> is 256.
        /// </remarks>
        /// <param name="length">Id string length</param>
        /// <returns>Generated id</returns>
        public static string Generate(uint length)
        {
            if (length == 0 || length > 256) { throw new ArgumentOutOfRangeException(nameof(length)); }

            var random = new Random((int)DateTimeOffset.UtcNow.Ticks % int.MaxValue);
            var sb = new StringBuilder();

            for (int i = 0; i < length; i++)
            {
                sb.Append(Alphabet[random.Next(0, Alphabet.Length)]);
            }

            return sb.ToString();
        }
    }
}
