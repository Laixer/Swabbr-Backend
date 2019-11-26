using Microsoft.Azure.Cosmos.Table;
using System;

namespace Swabbr.Infrastructure.Data.Entities
{
    /// <summary>
    /// Represents a like (love-it) given to a vlog.
    /// </summary>
    public class VlogLikeEntity : TableEntity
    {
        public VlogLikeEntity()
        {

        }

        public VlogLikeEntity(string vlogId, string userId)
        {
            PartitionKey = vlogId;
            RowKey = userId;
        }

        /// <summary>
        /// Id of the vlog that was given a like.
        /// </summary>
        public string VlogId { get; set; }

        /// <summary>
        /// Id of the user that created the like.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// The time at which the user liked the vlog.
        /// </summary>
        public DateTime TimeCreated { get; set; }
    }
}
