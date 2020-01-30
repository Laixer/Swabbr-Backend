using Newtonsoft.Json;
using Swabbr.Core.Entities;
using System;
using System.Collections.Generic;

namespace Swabbr.Api.ViewModels
{
    public class VlogOutputModel
    {
        /// <summary>
        /// Id of the vlog.
        /// </summary>
        [JsonProperty("vlogId")]
        public Guid VlogId { get; set; }

        /// <summary>
        /// Id of the user who created the vlog.
        /// </summary>
        [JsonProperty("userId")]
        public Guid UserId { get; set; }

        //TODO Comment
        [JsonProperty("downloadUrl")]
        public string DownloadUrl { get; set; }

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

        public static implicit operator VlogOutputModel(Vlog vlog)
        {
            return new VlogOutputModel
            {
                VlogId = vlog.VlogId,
                UserId = vlog.UserId,
                DownloadUrl = vlog.DownloadUrl,
                DateStarted = vlog.DateStarted,
                IsLive = vlog.IsLive,
                IsPrivate = vlog.IsPrivate,
                Likes = vlog.Likes,
            };
        }
    }
}