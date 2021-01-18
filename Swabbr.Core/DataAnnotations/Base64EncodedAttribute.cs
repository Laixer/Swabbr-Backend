using Swabbr.Core.Helpers;
using System;
using System.ComponentModel.DataAnnotations;

namespace Swabbr.Core.DataAnnotations
{
    /// <summary>
    ///     Attribute validating that some string is base 64 encoded.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = false)]
    public class Base64EncodedAttribute : ValidationAttribute
    {
        /// <summary>
        ///     Checks if the value is null or base 64 encoded.
        /// </summary>
        /// <remarks>
        ///     This only fully validates the string if we 
        ///     are in debug mode.
        /// </remarks>
        /// <param name="value">The item to check.</param>
        public override bool IsValid(object value)
            => value is null || (value is string str && str.Length > 0 && Base64EncodedHelper.IsBase64Encoded(str));

        /// <summary>
        ///     Formats the error message when validation fails.
        /// </summary>
        /// <param name="name">Field name.</param>
        /// <returns>Formatted error message.</returns>
        public override string FormatErrorMessage(string name)
            => $"The {name} field is not a valid base 64 encoded string.";
    }
}
