using Newtonsoft.Json;
using Swabbr.Api.ViewModels.Enums;
using System.ComponentModel.DataAnnotations;

namespace Swabbr.Api.ViewModels
{

    /// <summary>
    /// Input model for changing the user settings.
    /// </summary>
    public class UserSettingsInputModel
    {

        /// <summary>
        /// Indicates whether the profile of the user is publicly visible to other users.
        /// </summary>
        [JsonProperty("isPrivate")]
        [Required]
        public bool IsPrivate { get; set; }

        /// <summary>
        /// The maximum amount of times the user should be reminded to record a vlog through push notifications.
        /// </summary>
        [JsonProperty("dailyVlogRequestLimit")]
        [Required]
        [Range(0, 100)]
        public uint DailyVlogRequestLimit { get; set; }

        /// <summary>
        /// Determines how follow requests are processed for the user.
        /// </summary>
        [JsonProperty("followMode")]
        [Required]
        public FollowModeModel FollowMode { get; set; }

    }

}
