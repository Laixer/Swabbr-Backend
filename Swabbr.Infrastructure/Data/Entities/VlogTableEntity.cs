using Microsoft.Azure.Cosmos.Table;
using System;

namespace Swabbr.Infrastructure.Data.Entities
{
    /// <summary>
    /// Represents the storage data for a single vlog.
    /// </summary>
    public class VlogTableEntity : TableEntity
    {
        /// <summary>
        /// Unique identifier.
        /// </summary>
        public Guid VlogId { get; set; }

        /// <summary>
        /// Unique identifier of the livestream bound to this vlog.
        /// </summary>
        public string LivestreamId { get; set; }

        /// <summary>
        /// Download URL of the playback recording.
        /// </summary>
        public string DownloadUrl { get; set; }

        /// <summary>
        /// Id of the user who created the vlog.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Indicates if the vlog should be publicly available to other users.
        /// </summary>
        public bool IsPrivate { get; set; }

        /// <summary>
        /// Indicates whether the vlog is currently live or not.
        /// </summary>
        public bool IsLive { get; set; }

        /// <summary>
        /// The date at which the recording of the vlog started.
        /// </summary>
        public DateTime DateStarted { get; set; }

        // TODO: Add Metadata from Media Service to model?
        /// <summary>
        /// Metadata from the Media Service.
        /// </summary>
        public string MediaServiceData { get; set; }
    }
}