﻿using Newtonsoft.Json;
using Swabbr.Core.Enums;
using System;

namespace Swabbr.Core.Models
{
    /// <summary>
    /// Represents an active follow request between two users.
    /// </summary>
    public class FollowRequest : Entity
    {
        /// <summary>
        /// Id of the user that initiated the follow request.
        /// </summary>
        [JsonProperty("requesterId")]
        public string RequesterId { get; set; }

        /// <summary>
        /// Id of the user that should receive the follow request.
        /// </summary>
        [JsonProperty("receiverId")]
        public string ReceiverId { get; set; }

        /// <summary>
        /// Current status of the follow request.
        /// </summary>
        [JsonProperty("status")]
        public FollowRequestStatus Status { get; set; }

        /// <summary>
        /// Timestamp of when the request was initiated.
        /// </summary>
        [JsonProperty("timeCreated")]
        public DateTime TimeCreated { get; set; }
    }
}