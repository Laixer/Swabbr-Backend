using Microsoft.Azure.Cosmos.Table;
using System;

namespace Swabbr.Infrastructure.Data.Entities
{
    /// <summary>
    /// Represents a like (love-it) given to a vlog.
    /// </summary>
    public class VlogLikeTableEntity : TableEntity
    {
        /// <summary>
        /// Id of the vlog like.
        /// </summary>
        public Guid VlogLikeId { get; set; }

        /// <summary>
        /// Id of the vlog that was given a like.
        /// </summary>
        public Guid VlogId { get; set; }

        /// <summary>
        /// Id of the user that created the like.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// The time at which the user liked the vlog.
        /// </summary>
        public DateTime TimeCreated { get; set; }
    }
}