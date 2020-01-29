using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Swabbr.Api.ViewModels
{
    /// <summary>
    /// Represents an input model provided by the client for authenticating a user.
    /// </summary>
    public class UserAuthenticationInputModel
    {
        /// <summary>
        /// Email address input.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [JsonProperty("email", Required = Required.DisallowNull)]
        [EmailAddress, DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        /// <summary>
        /// Password input.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [JsonProperty("password", Required = Required.DisallowNull)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        /// <summary>
        /// Remember login option.
        /// </summary>
        [JsonProperty("rememberMe")]
        public bool RememberMe { get; set; } = false;
    }
}