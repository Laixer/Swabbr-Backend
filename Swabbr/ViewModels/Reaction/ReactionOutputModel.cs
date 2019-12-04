using Newtonsoft.Json;
using System;

namespace Swabbr.Api.ViewModels
{
    public class ReactionOutputModel
    {
        [JsonProperty("reactionId")]
        public Guid ReactionId { get; set; }

        /// <summary>
        /// Id of the user by whom this reaction was created.
        /// </summary>
        [JsonProperty("userId")]
        public Guid UserId { get; set; }

        /// <summary>
        /// Id of the vlog the reaction responds to.
        /// </summary>
        [JsonProperty("vlogId")]
        public Guid VlogId { get; set; }

        /// <summary>
        /// The moment at which the reaction was posted.
        /// </summary>
        [JsonProperty("datePosted")]
        public DateTime DatePosted { get; set; }

        /// <summary>
        /// Indicates whether this reaction is public or private.
        /// </summary>
        [JsonProperty("isPrivate")]
        public bool IsPrivate { get; set; }

        /// <summary>
        /// Metadata from the Media Service
        /// </summary>
        [JsonProperty("mediaServiceData")]
        public object MediaServiceData { get; set; }
    }
}