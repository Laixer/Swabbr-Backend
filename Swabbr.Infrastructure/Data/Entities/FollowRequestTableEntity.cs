using Microsoft.Azure.Cosmos.Table;
using Swabbr.Core.Enums;
using System;

namespace Swabbr.Infrastructure.Data.Entities
{
    /// <summary>
    /// Represents the current status of a follow relationship between two users.
    /// </summary>
    public class FollowRequestTableEntity : TableEntity
    {
        /// <summary>
        /// Unique identifier.
        /// </summary>
        public Guid FollowRequestId { get; set; }

        /// <summary>
        /// Id of the user that should receive the follow request.
        /// </summary>
        public Guid ReceiverId { get; set; }

        /// <summary>
        /// Id of the user that has initiated the follow request.
        /// </summary>
        public Guid RequesterId { get; set; }

        /// <summary>
        /// Current status of the follow request.
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Timestamp of when the request was initiated.
        /// </summary>
        public DateTime TimeCreated { get; set; }
    }
}