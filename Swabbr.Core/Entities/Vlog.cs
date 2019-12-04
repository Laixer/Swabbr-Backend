using System;
using System.Collections.Generic;

namespace Swabbr.Core.Entities
{
    /// <summary>
    /// A vlog created by a user.
    /// </summary>
    public class Vlog : EntityBase
    {
        /// <summary>
        /// Unique identifier.
        /// </summary>
        public Guid VlogId { get; set; }

        /// <summary>
        /// Id of the user who created the vlog.
        /// </summary>
        public Guid UserId { get; set; }

        //TODO: Is the duration of the vlog available from the media service metadata or should this be stored seperately?
        /// <summary>
        /// The duration of the vlog.
        /// </summary>
        public uint Duration { get; set; }

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

        /// <summary>
        /// Likes given to this vlog by users.
        /// </summary>
        public List<VlogLike> Likes { get; set; }

        // TODO: Add Metadata from Media Service to model?
        /// <summary>
        /// Metadata from the Media Service.
        /// </summary>
        public string MediaServiceData { get; set; }
    }
}