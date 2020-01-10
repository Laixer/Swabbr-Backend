using Microsoft.Azure.Cosmos.Table;
using System;

namespace Swabbr.Infrastructure.Data.Entities
{
    /// <summary>
    /// Represents the storage data for a WSC livestream.
    /// </summary>
    public class LivestreamTableEntity : TableEntity
    {
        /// <summary>
        /// Unique identifier of the Wowza Streaming Cloud Livestream.
        /// </summary>
        public string LivestreamId { get; set; }

        /// <summary>
        /// Id of the user this stream belongs to when it is not available.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Name of the stream.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Location of the broadcasting endpoint.
        /// </summary>
        public string BroadcastLocation { get; set; }

        /// <summary>
        /// Indicates availability of the stream.
        /// If it is not available, it is being used for broadcasting by a user.
        /// </summary>
        public bool Available { get; set; }

        /// <summary>
        /// HLS playback URL of the stream.
        /// </summary>
        public Uri PlaybackUrl { get; set; }

        /// <summary>
        /// Date and time this livestream was created externally through the livestreaming service.
        /// </summary>
        public DateTimeOffset CreatedAt { get; set; }

        /// <summary>
        /// Date and time this livestream was last updated externally through the livestreaming service.
        /// </summary>
        public DateTimeOffset UpdatedAt { get; set; }
    }
}