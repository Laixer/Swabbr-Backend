using Swabbr.Core.Types;
using System.ComponentModel.DataAnnotations;

namespace Swabbr.Api.DataTransferObjects
{
    /// <summary>
    ///     DTO for logging a user in.
    /// </summary>
    public record UserLoginDto
    {
        /// <summary>
        ///     User email.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public string Email { get; init; }

        /// <summary>
        ///     User password.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public string Password { get; init; }

        /// <summary>
        ///     Device handle.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public string Handle { get; init; }

        /// <summary>
        ///     Platform on which the device can receive push notifications.
        /// </summary>
        [Required]
        public PushNotificationPlatform PushNotificationPlatform { get; init; }
    }
}
