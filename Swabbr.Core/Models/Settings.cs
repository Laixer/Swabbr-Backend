﻿using Newtonsoft.Json;
using Swabbr.Core.Enums;

namespace Swabbr.Core.Models
{
    /// <summary>
    /// Personal settings and preferences for a user.
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// The maximum amount of times the user should be reminded
        /// to record a vlog through push notifications.
        /// </summary>
        [JsonProperty("dailyVlogRequestLimit")]
        public byte DailyVlogRequestLimit { get; set; }

        /// <summary>
        /// Determines how follow requests are processed for the user.
        /// </summary>
        [JsonProperty("followMode")]
        public FollowMode FollowMode { get; set; }
    }
}