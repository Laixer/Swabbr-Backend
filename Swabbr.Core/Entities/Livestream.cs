using Swabbr.Core.Enums;
using System;

namespace Swabbr.Core.Entities
{

    /// <summary>
    /// Represents a livestream entity.
    /// </summary>
    public class Livestream : EntityBase<Guid>
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
        public DateTimeOffset CreateDate { get; set; }

        /// <summary>
        /// Update date.
        /// </summary>
        public DateTimeOffset UpdateDate { get; set; }

        /// <summary>
        /// Indicates the current status of the livestream on our external platform.
        /// </summary>
        public LivestreamState LivestreamState { get; set; }

        /// <summary>
        /// Represents the trigger minute at which this livestream was created.
        /// </summary>
        public int UserTriggerMinute { get; set; }

    }

}
