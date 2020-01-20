using System;

namespace Swabbr.Core.Entities
{
    public class Livestream : EntityBase
    {
        /// <summary>
        /// Unique identifier of the livestream.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Unique identifier of the user this livestream temporarily belongs to.
        /// </summary>
        public Guid UserId { get; set; }

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