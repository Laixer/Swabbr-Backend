using Newtonsoft.Json;

namespace Swabbr.Api.ViewModels
{
    public class UserLoginInputModel
    {
        /// <summary>
        /// Email address.
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; set; }

        /// <summary>
        /// Hashed password of the user.
        /// </summary>
        [JsonProperty("password")]
        public string Password { get; set; }
    }
}
