using System;
using System.ComponentModel.DataAnnotations;


namespace Swabbr.Core.DataAnnotations
{
    /// <summary>
    ///     Attribute forcing a Guid to be not empty.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = false)]
    public class NonEmptyGuidAttribute : ValidationAttribute
    {
        /// <summary>
        ///     Checks if the value is null or a non-empty guid.
        /// </summary>
        /// <param name="value">The item to check.</param>
        public override bool IsValid(object value)
            => value is null || (value is Guid g && g != Guid.Empty);

        /// <summary>
        ///     Formats the error message when validation fails.
        /// </summary>
        /// <param name="name">Field name.</param>
        /// <returns>Formatted error message.</returns>
        public override string FormatErrorMessage(string name)
            => $"The {name} field can't be an empty Guid.";
    }
}
