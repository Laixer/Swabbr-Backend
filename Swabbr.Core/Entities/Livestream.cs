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
        /// Indicates if the livestream is currently being used.
        /// </summary>
        public bool Available { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }
    }
}