using System;

namespace Swabbr.Core.Entities
{

    /// <summary>
    /// A vlog created by a user.
    /// TODO This needs some work
    /// </summary>
    public class Vlog : EntityBase<Guid>
    {

        /// <summary>
        /// Download url of the recording.
        /// </summary>
        public Uri DownloadUrl { get; set; }

        /// <summary>
        /// Id of the user who created the vlog.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// References the <see cref="Livestream"/> on which this vlog was created.
        /// </summary>
        public Guid LivestreamId { get; set; }

        /// <summary>
        /// Indicates if the vlog should be publicly available to other users.
        /// </summary>
        public bool IsPrivate { get; set; }

        /// <summary>
        /// The date at which the recording of the vlog started.
        /// </summary>
        public DateTimeOffset StartDate { get; set; }

        ///// <summary>
        ///// Metadata from the Media Service.
        ///// </summary>
        //public string MediaServiceData { get; set; }

    }

}
