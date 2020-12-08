using System.ComponentModel.DataAnnotations;

namespace Swabbr.Api.DataTransferObjects
{
    /// <summary>
    ///     DTO for registering a user.
    /// </summary>
    /// <remarks>
    ///     This extends <see cref="UserUpdateDto"/> to ensure any
    ///     user properties which can be modified can also be sent
    ///     along with the registration process.
    /// </remarks>
    public class UserRegistrationDto : UserUpdateDto
    {
        /// <summary>
        ///     Nickname to display for the user.
        /// </summary>
        /// <remarks>
        ///     This hides the base member to require the nickname.
        /// </remarks>
        [Required(AllowEmptyStrings = false)]
        public new string Nickname { get; set; }

        /// <summary>
        ///     User registration email.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public string Email { get; set; }

        /// <summary>
        ///     User password.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public string Password { get; set; }
    }
}
