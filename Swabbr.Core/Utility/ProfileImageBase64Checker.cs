using System;
using System.Collections.Generic;
using System.Text;

namespace Swabbr.Core.Utility
{

    /// <summary>
    /// Contains functionality to validate a base64 encoded profile image string.
    /// </summary>
    public static class ProfileImageBase64Checker
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
                string decoded = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(input));
                string encoded = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(decoded));
                return input.Equals(encoded, StringComparison.InvariantCultureIgnoreCase);
            }
            catch (Exception)
            {
                return false;
            }
        }

    }

}
