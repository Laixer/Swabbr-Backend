using Newtonsoft.Json;
using Swabbr.Core.Entities;
using Swabbr.Core.Enums;
using System;

namespace Swabbr.Api.ViewModels
{
    public class FollowRequestOutputModel
    {
        /// <summary>
        /// Unique identifier.
        /// </summary>
        [JsonProperty("followRequestId")]
        public Guid FollowRequestId { get; set; }

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
        public FollowRequestStatus Status { get; set; }

        /// <summary>
        /// Timestamp of when the request was initiated.
        /// </summary>
        [JsonProperty("timeCreated")]
        public DateTime TimeCreated { get; set; }

        public static implicit operator FollowRequestOutputModel(FollowRequest followRequest)
            => new FollowRequestOutputModel
            {
                FollowRequestId = followRequest.FollowRequestId,
                ReceiverId = followRequest.ReceiverId,
                RequesterId = followRequest.RequesterId,
                Status = followRequest.Status,
                TimeCreated = followRequest.TimeCreated
            };
    }
}