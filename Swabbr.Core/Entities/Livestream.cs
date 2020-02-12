using System;

namespace Swabbr.Core.Entities
{

    /// <summary>
    /// Represents a livestream entity.
    /// </summary>
    public class Livestream : EntityBase
    {

        /// <summary>
        /// External unique identifier for the service on which this livestream is hosted.
        /// </summary>
        public string ExternalId { get; set; }

        /// <summary>
        /// Unique identifier of the user this livestream temporarily belongs to.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Unique identifier of the vlog this livestream temporarily belongs to.
        /// </summary>
        public Guid VlogId { get; set; }

        /// <summary>
        /// Name of the livestream.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Broadcasting location of the livestream.
        /// </summary>
        public string BroadcastLocation { get; set; }

        /// <summary>
        /// Indicates whether the livestream is currently active or not.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Creation date.
        /// </summary>
        public DateTimeOffset CreatedAt { get; set; }

        /// <summary>
        /// Update date.
        /// </summary>
        public DateTimeOffset UpdatedAt { get; set; }

    }

}
