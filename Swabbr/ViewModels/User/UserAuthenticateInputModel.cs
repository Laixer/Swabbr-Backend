using Newtonsoft.Json;
using Swabbr.Core.Entities;

namespace Swabbr.Api.ViewModels
{
    /// <summary>
    /// Represents an input model provided by the client for authenticating a user.
    /// </summary>
    public class UserAuthenticateInputModel
    {
        /// <summary>
        /// Email.
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; set; }

        /// <summary>
        /// Password input.
        /// </summary>
        [JsonProperty("password")]
        public string Password { get; set; }

        /// <summary>
        /// Remember login option.
        /// </summary>
        [JsonProperty("rememberMe")]
        public bool RememberMe { get; set; }

        public static implicit operator User(UserAuthenticateInputModel user)
            => new User
            {
                Email = user.Email,
                // TODO Password should be hashed (this conversion is unnecessary)
                PasswordHash = user.Password
            };
    }
}