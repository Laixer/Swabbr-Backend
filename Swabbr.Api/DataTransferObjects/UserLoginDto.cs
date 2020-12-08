using Swabbr.Core.Types;
using System.ComponentModel.DataAnnotations;

namespace Swabbr.Api.DataTransferObjects
{
    /// <summary>
    ///     DTO for logging a user in.
    /// </summary>
    public class UserLoginDto
    {
        /// <summary>
        ///     User email.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public string Email { get; set; }

        /// <summary>
        ///     User password.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public string Password { get; set; }

        /// <summary>
        ///     Device handle.
        /// </summary>
        public string Handle { get; set; }

        /// <summary>
        ///     Platform on which the device can receive push notifications.
        /// </summary>
        public PushNotificationPlatform PushNotificationPlatform { get; set; }
    }
}
