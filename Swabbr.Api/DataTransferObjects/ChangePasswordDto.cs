using System.ComponentModel.DataAnnotations;

namespace Swabbr.Api.DataTransferObjects
{
    /// <summary>
    ///     DTO for changing a password.
    /// </summary>
    public record ChangePasswordDto
    {
        /// <summary>
        ///     Old user password.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public string CurrentPassword { get; init; }

        /// <summary>
        ///     New user password.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public string NewPassword { get; init; }
    }
}
