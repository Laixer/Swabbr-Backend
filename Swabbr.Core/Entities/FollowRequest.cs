using Swabbr.Core.Enums;
using System;

namespace Swabbr.Core.Entities
{
    /// <summary>
    /// Represents an active follow request between two users.
    /// </summary>
    public class FollowRequest : EntityBase
    {
        /// <summary>
        /// Unique identifier.
        /// </summary>
        public Guid FollowRequestId { get; set; }

        /// <summary>
        /// Id of the user that initiated the follow request.
        /// </summary>
        public Guid RequesterId { get; set; }

        /// <summary>
        /// Id of the user that should receive the follow request.
        /// </summary>
        public Guid ReceiverId { get; set; }

        /// <summary>
        /// Current status of the follow request.
        /// </summary>
        public FollowRequestStatus Status { get; set; }

        /// <summary>
        /// Timestamp of when the request was initiated.
        /// </summary>
        public DateTime TimeCreated { get; set; }
    }
}