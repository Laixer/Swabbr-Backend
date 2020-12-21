using Bogus;

namespace Swabbr.Testing.Extensions
{
    /// <summary>
    ///     Contains faker extension functionality.
    /// </summary>
    public static class FakerExtensions
    {
        /// <summary>
        ///     Generates a random password.
        /// </summary>
        /// <param name="randomizer">Randomizer object.</param>
        /// <param name="length">Password length.</param>
        /// <returns>Randomized password.</returns>
        public static string Password(this Randomizer randomizer, int length = 12)
            => randomizer.String2(length, "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ!@#$%^&*()_-==+`~");
    }
}
