using Newtonsoft.Json;
using System;

namespace Swabbr.Core.Models
{
    /// <summary>
    /// A vlog created by a user.
    /// </summary>
    public class VlogItem : Entity
    {
        /// <summary>
        /// Id of the user who created the vlog.
        /// </summary>
        [JsonProperty("userId")]
        public Guid UserId { get; set; }

        //TODO: Is the duration of the vlog available from the media service metadata?
        /// <summary>
        /// The duration of the vlog.
        /// </summary>
        [JsonProperty("duration")]
        public uint Duration { get; set; }

        /// <summary>
        /// Indicates if the vlog should be publicly available to other users.
        /// </summary>
        [JsonProperty("isPrivate")]
        public bool IsPrivate { get; set; }

        /// <summary>
        /// Indicates whether the vlog is currently live or not.
        /// </summary>
        [JsonProperty("isLive")]
        public bool IsLive { get; set; }

        /// <summary>
        /// The date at which the recording of the vlog started.
        /// </summary>
        [JsonProperty("dateStarted")]
        public DateTime DateStarted { get; set; }

        // TODO: Add Metadata from Media Service to model?
    }
}