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
        public Guid Id { get; set; }

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
        public DateTimeOffset TimeCreated { get; set; }

        // TODO THOMAS This is mapping in the wrong location, make separate object for this
        public static FollowRequestOutputModel Parse(FollowRequest followRequest)
        {
            return new FollowRequestOutputModel
            {
                Id = followRequest.Id,
                ReceiverId = followRequest.ReceiverId,
                RequesterId = followRequest.RequesterId,
                Status = followRequest.Status,
                TimeCreated = followRequest.TimeCreated
            };
        }
    }
}