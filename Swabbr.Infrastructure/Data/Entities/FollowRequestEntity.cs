using Microsoft.Azure.Cosmos.Table;
using Swabbr.Core.Enums;
using System;
using System.Collections.Generic;

namespace Swabbr.Infrastructure.Data.Entities
{
    /// <summary>
    /// Represents the current status of a follow relationship between two users.
    /// </summary>
    public class FollowRequestEntity : TableEntity
    {
        public FollowRequestEntity()
        {

        }

        /// <summary>
        /// Entity Id.
        /// </summary>
        public string FollowRequestId 
        {
            get => RowKey;
            set => RowKey = value;
        }

        /// <summary>
        /// Id of the user that should receive the follow request.
        /// </summary>
        public string ReceiverId 
        {
            get => PartitionKey;
            set => PartitionKey = value;
        }

        /// <summary>
        /// Id of the user that has initiated the follow request.
        /// </summary>
        public string RequesterId { get; set; }

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
