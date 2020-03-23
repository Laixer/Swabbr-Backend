using Newtonsoft.Json;
using Swabbr.Api.ViewModels.Enums;
using System;

namespace Swabbr.Api.ViewModels
{

    /// <summary>
    /// Represents a single user settings object.
    /// </summary>
    public class UserSettingsOutputModel
    {

        /// <summary>
        /// Id of the user these settings belong to.
        /// </summary>
        [JsonProperty("userId")]
        public Guid UserId { get; set; }

        /// <summary>
        /// Indicates whether the profile of the user is publicly visible to other users.
        /// </summary>
        [JsonProperty("isPrivate")]
        public bool IsPrivate { get; set; }

        /// <summary>
        /// The maximum amount of times the user should be reminded to record a vlog through push notifications.
        /// </summary>
        [JsonProperty("dailyVlogRequestLimit")]
        public int DailyVlogRequestLimit { get; set; }

        /// <summary>
        /// Determines how follow requests are processed for the user.
        /// </summary>
        [JsonProperty("followMode")]
        public string FollowMode { get; set; }

    }

}
