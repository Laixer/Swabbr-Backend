﻿using Newtonsoft.Json;
using Swabbr.Api.ViewModels.Enums;
using System;

namespace Swabbr.Api.ViewModels
{

    /// <summary>
    /// Represents a single follow request.
    /// </summary>
    public class FollowRequestOutputModel
    {

        /// <summary>
        /// Id of the user that initiated the follow request.
        /// </summary>
        [JsonProperty("requesterId")]
        public Guid RequesterId { get; set; }

        /// <summary>
        /// Id of the user that should receive the follow request.
        /// </summary>
        [JsonProperty("receiverId")]
        public Guid ReceiverId { get; set; }

        /// <summary>
        /// Current status of the follow request.
        /// </summary>
        [JsonProperty("status")]
        public FollowRequestStatusModel Status { get; set; }

        /// <summary>
        /// Timestamp of when the request was initiated.
        /// </summary>
        [JsonProperty("timeCreated")]
        public DateTimeOffset TimeCreated { get; set; }

    }

}
