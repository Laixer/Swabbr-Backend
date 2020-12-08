using System.ComponentModel.DataAnnotations;

namespace Swabbr.Api.DataTransferObjects
{
    /// <summary>
    ///     DTO for changing a password.
    /// </summary>
    public class ChangePasswordDto
    {
        /// <summary>
        ///     Old user password.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public string CurrentPassword { get; set; }

        /// <summary>
        ///     New user password.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public string NewPassword { get; set; }
    }
}
