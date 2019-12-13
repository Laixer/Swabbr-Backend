using Newtonsoft.Json;
using Swabbr.Core.Entities;
using Swabbr.Core.Enums;
using System;

namespace Swabbr.Api.ViewModels
{
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
        public byte DailyVlogRequestLimit { get; set; }

        /// <summary>
        /// Determines how follow requests are processed for the user.
        /// </summary>
        [JsonProperty("followMode")]
        public FollowMode FollowMode { get; set; }

        public static implicit operator UserSettingsOutputModel(UserSettings entity)
        {
            return new UserSettingsOutputModel
            {
                UserId = entity.UserId,
                FollowMode = entity.FollowMode,
                DailyVlogRequestLimit = entity.DailyVlogRequestLimit,
                IsPrivate = entity.IsPrivate
            };
        }
    }
}