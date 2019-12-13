using Newtonsoft.Json;
using Swabbr.Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace Swabbr.Api.ViewModels
{
    /// <summary>
    /// Represents an input model provided by the client for authenticating a user.
    /// </summary>
    public class UserAuthenticationInputModel
    {
        /// <summary>
        /// Email.
        /// </summary>
        [Required, JsonProperty("email")]
        [EmailAddress, DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        /// <summary>
        /// Password input.
        /// </summary>
        [Required, JsonProperty("password")]
        public string Password { get; set; }

        /// <summary>
        /// Remember login option.
        /// </summary>
        [JsonProperty("rememberMe")]
        public bool RememberMe { get; set; }
    }
}