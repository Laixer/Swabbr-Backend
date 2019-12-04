using Swabbr.Core.Enums;
using System;

namespace Swabbr.Infrastructure.Data.Entities
{
    /// <summary>
    /// Personal settings and preferences for a user.
    /// </summary>
    public class UserSettingsEntity
    {
        public UserSettingsEntity()
        {
        }

        /// <summary>
        /// Unique identifier of the user these settings belong to.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// The maximum amount of times the user should be reminded
        /// to record a vlog through push notifications.
        /// </summary>
        public byte DailyVlogRequestLimit { get; set; }

        /// <summary>
        /// Determines how follow requests are processed for the user.
        /// </summary>
        public FollowMode FollowMode { get; set; }
    }
}