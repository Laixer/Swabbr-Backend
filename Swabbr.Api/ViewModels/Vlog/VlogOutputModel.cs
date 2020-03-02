using Newtonsoft.Json;
using Swabbr.Api.ViewModels.Vlog;
using Swabbr.Api.ViewModels.VlogLike;
using Swabbr.Core.Entities;
using System;
using System.Collections.Generic;

namespace Swabbr.Api.ViewModels.Vlog
{

    /// <summary>
    /// Represents a single vlog.
    /// </summary>
    public class VlogOutputModel
    {

        /// <summary>
        /// Id of the vlog.
        /// </summary>
        [JsonProperty("vlogId")]
        public Guid Id { get; set; }

        /// <summary>
        /// Id of the user who created the vlog.
        /// </summary>
        [JsonProperty("userId")]
        public Guid UserId { get; set; }

        /// <summary>
        /// Download URL of the recording of the livestream.
        /// </summary>
        [JsonProperty("downloadUrl")]
        public Uri DownloadUrl { get; set; }

        /// <summary>
        /// Indicates if the vlog should be publicly available to other users.
        /// </summary>
        [JsonProperty("isPrivate")]
        public bool IsPrivate { get; set; }

        // TODO Evaluate
        ///// <summary>
        ///// Indicates whether the vlog is currently live or not.
        ///// </summary>
        //[JsonProperty("isLive")]
        //public bool IsLive { get; set; }

        /// <summary>
        /// The date at which the recording of the vlog started.
        /// </summary>
        [JsonProperty("dateStarted")]
        public DateTimeOffset DateStarted { get; set; }

    }

}
