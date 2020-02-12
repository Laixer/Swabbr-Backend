using System;

namespace Swabbr.Core.Entities
{
    public class Livestream : EntityBase
    {
        /// <summary>
        /// Unique identifier of the livestream.
        /// </summary>
        /// TODO THOMAS Don't use strings as id --> at the moment the database even uses Guid, which then are converted back to string!
        public string Id { get; set; }

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

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }
    }
}