using Newtonsoft.Json;
using Swabbr.Api.ViewModels.Enums;
using System.ComponentModel.DataAnnotations;

namespace Swabbr.Api.ViewModels.User
{

    /// <summary>
    /// Represents an input model provided by the client for authenticating a user.
    /// </summary>
    public class UserLoginInputModel
    {

        /// <summary>
        /// Email address input.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [EmailAddress, DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        /// <summary>
        /// Password input.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        /// <summary>
        /// Remember login option.
        /// </summary>
        public bool RememberMe { get; set; } = false;

        /// <summary>
        /// Indicates which <see cref="PushNotificationPlatform"/> we are on.
        /// </summary>
        [Required]
        public PushNotificationPlatformModel? PushNotificationPlatform { get; set; }

        /// <summary>
        /// PNS handle.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public string Handle { get; set; }

    }

}
