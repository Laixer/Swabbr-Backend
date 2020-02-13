using Newtonsoft.Json;
using System.Collections.Generic;
using System.Security.Claims;

namespace Swabbr.Api.ViewModels
{
    public sealed class UserAuthenticationOutputModel
    {
        /// <summary>
        /// Application access token of the authenticated user.
        /// </summary>
        [JsonProperty("token")]
        public string Token { get; set; }

        //TODO ...
        /// <summary>
        /// The amount of seconds the token is valid.
        /// </summary>
        [JsonProperty("tokenValid")]
        public int TokenValid { get; set; }

        /// <summary>
        /// Claims of the authenticated user.
        /// </summary>
        [JsonProperty("claims")]
        public IEnumerable<Claim> Claims { get; set; }

        /// <summary>
        /// Application roles of the user.
        /// </summary>
        [JsonProperty("roles")]
        public IEnumerable<string> Roles { get; set; }

        /// <summary>
        /// Information about the authenticated user.
        /// </summary>
        [JsonProperty("user")]
        public UserOutputModel User { get; set; }

        /// <summary>
        /// Settings of the authenticated user.
        /// </summary>
        [JsonProperty("userSettings")]
        public UserSettingsOutputModel UserSettings { get; set; }
    }
}