using Newtonsoft.Json;
using Swabbr.Core.Models;
using System;
using System.Collections.Generic;

namespace Swabbr.Core.Documents
{
    /// <summary>
    /// Represents a database document for a single vlog.
    /// </summary>
    public class VlogDocument : Entity
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

        /// <summary>
        /// Likes given to this vlog by users.
        /// </summary>
        [JsonProperty("likes")]
        public List<VlogLike> Likes { get; set; }

        // TODO: Add Metadata from Media Service to model?
    }
}
