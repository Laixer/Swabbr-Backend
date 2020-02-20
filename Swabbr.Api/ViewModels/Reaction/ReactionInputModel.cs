using Newtonsoft.Json;
using System;

namespace Swabbr.Api.ViewModels
{

    /// <summary>
    /// Input model for placing a reaction.
    /// </summary>
    public class ReactionInputModel
    {

        /// <summary>
        /// Id of the vlog the reaction responds to.
        /// </summary>
        [JsonProperty("vlogId")]
        public Guid VlogId { get; set; }

        /// <summary>
        /// Indicates whether this reaction is public or private.
        /// </summary>
        [JsonProperty("isPrivate")]
        public bool IsPrivate { get; set; }

    }

}
