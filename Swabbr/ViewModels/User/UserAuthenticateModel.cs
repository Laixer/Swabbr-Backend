using Newtonsoft.Json;
using Swabbr.Core.Entities;

namespace Swabbr.Api.ViewModels
{
    /// <summary>
    /// Represents an input model provided by the client for authenticating a user.
    /// </summary>
    public class UserAuthenticateModel
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

        public static implicit operator User(UserAuthenticateModel user)
        {
            return new User
            {
                Email = user.Email,
                Password = user.Password
            };
        }
    }
}