using Newtonsoft.Json;
using System;

namespace Swabbr.Api.ViewModels
{

    /// <summary>
    /// Represents a single reaction.
    /// </summary>
    public class ReactionOutputModel
    {

        /// <summary>
        /// Id of the reaction.
        /// </summary>
        [JsonProperty("reactionId")]
        public Guid Id { get; set; }

        /// <summary>
        /// Id of the user by whom this reaction was created.
        /// </summary>
        [JsonProperty("userId")]
        public Guid UserId { get; set; }

        /// <summary>
        /// Id of the vlog the reaction responds to.
        /// </summary>
        [JsonProperty("vlogId")]
        public Guid TargetVlogId { get; set; }

        /// <summary>
        /// The moment at which the reaction was posted.
        /// </summary>
        [JsonProperty("datePosted")]
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// Indicates whether this reaction is public or private.
        /// </summary>
        [JsonProperty("isPrivate")]
        public bool IsPrivate { get; set; }

    }

}
