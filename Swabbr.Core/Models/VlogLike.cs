using Newtonsoft.Json;
using System;

namespace Swabbr.Core.Models
{
    /// <summary>
    /// Represents a like (love-it) given to a vlog.
    /// </summary>
    public class VlogLike
    {
        /// <summary>
        /// Id of the vlog that was given a like.
        /// </summary>
        [JsonProperty("vlogId")]
        public string VlogId { get; set; }

        /// <summary>
        /// Id of the user that created the like.
        /// </summary>
        [JsonProperty("userId")]
        public string UserId { get; set; }

        /// <summary>
        /// The time at which the user liked the vlog.
        /// </summary>
        [JsonProperty("timeCreated")]
        public DateTime TimeCreated { get; set; }
    }
}