using Newtonsoft.Json;
using Swabbr.Core.Entities;
using Swabbr.Core.Enums;

namespace Swabbr.Api.ViewModels
{
    public class UserSettingsInputModel
    {
        /// <summary>
        /// Indicates whether the profile of the user is publicly visible to other users.
        /// </summary>
        [JsonProperty("isPrivate")]
        public bool IsPrivate { get; set; }

        /// <summary>
        /// The maximum amount of times the user should be reminded to record a vlog through push notifications.
        /// </summary>
        [JsonProperty("dailyVlogRequestLimit")]
        public uint DailyVlogRequestLimit { get; set; }

        /// <summary>
        /// Determines how follow requests are processed for the user.
        /// </summary>
        [JsonProperty("followMode")]
        public FollowMode FollowMode { get; set; }

        public static implicit operator UserSettings(UserSettingsInputModel entity)
        {
            return new UserSettings
            {
                FollowMode = entity.FollowMode,
                DailyVlogRequestLimit = entity.DailyVlogRequestLimit,
                IsPrivate = entity.IsPrivate
            };
        }
    }
}