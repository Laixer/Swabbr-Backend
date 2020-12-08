using System;
using System.Text;

namespace Swabbr.Core.Helpers
{

    /// <summary>
    /// Contains functionality to validate a base64 encoded profile image string.
    /// </summary>
    public static class ProfileImageBase64Helper
    {

        /// <summary>
        /// TODO Update
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsValid(string input)
        {
            if (input == null) { throw new ArgumentNullException(nameof(input)); }
            try
            {
                string decoded = Encoding.UTF8.GetString(Convert.FromBase64String(input));
                string encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(decoded));
                return input.Equals(encoded, StringComparison.InvariantCultureIgnoreCase);
            }
            catch (FormatException)
            {
                return false;
            }
            catch (DecoderFallbackException)
            {
                return false;
            }
            catch (EncoderFallbackException)
            {
                return false;
            }
        }

    }

}
