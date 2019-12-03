using Newtonsoft.Json;
using Swabbr.Core.Enums;

namespace Swabbr.Api.ViewModels
{
    public class FollowRequestInput
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
    }
}