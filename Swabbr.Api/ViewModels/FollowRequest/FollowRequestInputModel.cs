using Newtonsoft.Json;
using Swabbr.Core.Entities;
using Swabbr.Core.Enums;
using System;

namespace Swabbr.Api.ViewModels
{
    public class FollowRequestInputModel
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
        public FollowRequestStatus Status { get; set; }

        public static implicit operator FollowRequest(FollowRequestInputModel model)
        {
            return new FollowRequest
            {
                ReceiverId = model.ReceiverId,
                RequesterId = model.RequesterId,
                Status = model.Status
            };
        }
    }
}