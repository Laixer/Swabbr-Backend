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
    public record UserRegistrationDto : UserUpdateDto
    {
        /// <summary>
        ///     Nickname to display for the user.
        /// </summary>
        /// <remarks>
        ///     This hides the base member to make 
        ///     the nickname required.
        /// </remarks>
        [Required(AllowEmptyStrings = false)]
        public new string Nickname { get; init; }

        /// <summary>
        ///     User registration email.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [EmailAddress]
        public string Email { get; init; }

        /// <summary>
        ///     User password.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public string Password { get; init; }
    }
}
