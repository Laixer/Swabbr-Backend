using Newtonsoft.Json;
using System;

namespace Swabbr.Api.ViewModels
{
    public class ReactionUpdateInputModel
    {
        /// <summary>
        /// Id of the reaction.
        /// </summary>
        [JsonProperty("reactionId")]
        public Guid ReactionId { get; set; }

        /// <summary>
        /// Indicates whether this reaction is public or private.
        /// </summary>
        [JsonProperty("isPrivate")]
        public bool IsPrivate { get; set; }
    }
}
