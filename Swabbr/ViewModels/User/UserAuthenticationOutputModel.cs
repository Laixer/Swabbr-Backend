using Newtonsoft.Json;

namespace Swabbr.Api.ViewModels
{
    public sealed class UserAuthenticationOutputModel
    {
        /// <summary>
        /// Api access token of the authenticated user.
        /// </summary>
        [JsonProperty("accessToken")]
        public string AccessToken { get; set; }

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