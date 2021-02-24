using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Linq;
using System.Security.Cryptography;

namespace Swabbr.Core.Helpers
{
    /// <summary>
    ///     Used to hash confidential information.
    /// </summary>
    /// <remarks>
    ///     This was based on the Microsoft documentation for hashing passwords and
    ///     depends on the KeyDerivation package which is standalone. The way we
    ///     store our data and validate our hashes is based on the Identity Framework
    ///     Password Hasher.
    ///     https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/consumer-apis/password-hashing?view=aspnetcore-5.0
    ///     https://github.com/dotnet/AspNetCore/blob/main/src/Identity/Extensions.Core/src/PasswordHasher.cs
    /// </remarks>
    public static class SecureHashHelper
    {
        private const int SaltLengthInBits = 128;
        private const int HashLengthInBits = 256;
        private const int IterationCount = 10000;

        /// <summary>
        ///     Hash an input string using the KeyDerivation.Pbkdf2 algorithm.
        /// </summary>
        /// <param name="input">The input that should be hashed.</param>
        /// <returns>Base64 string of 128 bit salt string + hashed input.</returns>
        public static string Hash(string input)
        {
            // Generate a 128-bit salt using a secure PRNG
            byte[] salt = new byte[SaltLengthInBits / 8];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(salt);

            // Hash, combine with salt and return.
            return HashAndPack(salt, input);
        }

        /// <summary>
        ///     Checks if <paramref name="input"/> matches the provided
        ///     <paramref name="saltWithHash"/> using the salt stored in
        ///     the latter.
        /// </summary>
        /// <param name="saltWithHash">Base64 string of salt+hash.</param>
        /// <param name="input">The string to hash and check if it matches <paramref name="saltWithHash"/></param>
        public static bool DoesHashMatch(string saltWithHash, string input)
        {
            // Extract the salt
            var hashedBytes = Convert.FromBase64String(saltWithHash);
            if (hashedBytes.Length != (SaltLengthInBits / 8) + (HashLengthInBits / 8))
            {
                throw new ArgumentException("Invalid format for base64 salt+hash string we are checking against");
            }

            var salt = new byte[SaltLengthInBits / 8];
            Buffer.BlockCopy(hashedBytes, 0, salt, 0, salt.Length);

            return HashAndPack(salt, input) == saltWithHash;
        }

        /// <summary>
        ///     Derive a hash by using HMACSHA1.
        /// </summary>
        /// <returns>Base64 string of 128 bit salt string + hashed input.</returns>
        private static string HashAndPack(byte[] salt, string input) {
            var hash = KeyDerivation.Pbkdf2(
                password: input,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: IterationCount,
                numBytesRequested: HashLengthInBits / 8);

            // Combine salt + hash together.
            return Convert.ToBase64String(salt.Concat(hash).ToArray());
        }
    }
}
