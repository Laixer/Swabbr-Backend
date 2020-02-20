using System;
using System.Collections.Generic;

namespace Swabbr.Core.Entities
{

    /// <summary>
    /// A vlog created by a user.
    /// </summary>
    public class Vlog : EntityBase<Guid>
    {

        /// <summary>
        /// Unique identifier of the livestream bound to this vlog.
        /// </summary>
        public Guid LivestreamId { get; set; }

        /// <summary>
        /// Download url of the recording.
        /// </summary>
        public Uri DownloadUrl { get; set; }

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
        public DateTimeOffset DateStarted { get; set; }

        /// <summary>
        /// Likes given to this vlog by users.
        /// </summary>
        public IEnumerable<VlogLike> Likes { get; set; }

        /// <summary>
        /// Metadata from the Media Service.
        /// </summary>
        public string MediaServiceData { get; set; }

    }

}
