using Laixer.Utility.Extensions;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Swabbr.WowzaStreamingCloud.Tokens
{

    /// <summary>
    /// Generates HMAC tokens for Fastly.
    /// </summary>
    internal static class FastlyHmacGenerator
    {

        /// <summary>
        /// Generates a HMAC token for Fastly playback.
        /// </summary>
        /// <returns></returns>
        internal static string Generate(string fastlyStreamId, string sharedSecret)
        {
            fastlyStreamId.ThrowIfNullOrEmpty();
            sharedSecret.ThrowIfNullOrEmpty();

            var epochStart = DateTimeOffset.Now.ToUnixTimeSeconds();
            var lifetime = (long) TimeSpan.FromMinutes(10).TotalSeconds; // TODO Make config
            var epochEnd = epochStart + lifetime;

            var hashSource = $"exp={epochEnd}~stream_id={fastlyStreamId}";
            using (var hmac = new HMACSHA256(Encoding.ASCII.GetBytes(sharedSecret)))
            {
                var hashMessage = hmac.ComputeHash(Encoding.ASCII.GetBytes(hashSource));
                var hmacString = ToHexString(hashMessage);
                return $"hdnts=exp={epochEnd}~hmac={hmacString}";
            }
        }

        /// <summary>
        /// Converts a byte array to hex string.
        /// </summary>
        /// <param name="array">Byte array</param>
        /// <returns>Hext string</returns>
        private static string ToHexString(byte[] array)
        {
            StringBuilder hex = new StringBuilder(array.Length * 2);
            foreach (byte b in array)
            {
                hex.AppendFormat("{0:x2}", b);
            }
            return hex.ToString();
        }

    }

}
