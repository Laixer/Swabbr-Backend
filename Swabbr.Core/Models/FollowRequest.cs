﻿using Newtonsoft.Json;
using Swabbr.Core.Enums;
using System;

namespace Swabbr.Core.Models
{
    /// <summa0402ry>
    /// Represents an active follow request between two users.
    /// </summary>
    public class FollowRequest : Entity
    {
        /// <summary>
        /// Id of the user that initiated the follow request.
        /// </summary>
        public string RequesterId { get; set; }

        /// <summary>
        /// Id of the user that should receive the follow request.
        /// </summary>
        public string ReceiverId { get; set; }

        /// <summary>
        /// Current status of the follow request.
        /// </summary>
        public FollowRequestStatus Status { get; set; }

        /// <summary>
        /// Timestamp of when the request was initiated.
        /// </summary>
        public DateTime TimeCreated { get; set; }
    }
}