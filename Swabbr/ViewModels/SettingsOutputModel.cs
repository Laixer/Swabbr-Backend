using Newtonsoft.Json;
using Swabbr.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Swabbr.Api.ViewModels
{
    public class SettingsOutputModel
    {
        /// <summary>
        /// Id of the user these settings belong to.
        /// </summary>
        [JsonProperty("userId")]
        public string UserId { get; set; }

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
