using Microsoft.Azure.Cosmos.Table;
using System;

namespace Swabbr.Infrastructure.Data.Entities
{
    /// <summary>
    /// A reaction to a vlog.
    /// </summary>
    public class ReactionTableEntity : TableEntity
    {
        /// <summary>
        /// Unique identifier of the reaction.
        /// </summary>
        public Guid ReactionId { get; set; }

        /// <summary>
        /// Id of the user by whom this reaction was created.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Id of the vlog the reaction responds to.
        /// </summary>
        public Guid VlogId { get; set; }

        /// <summary>
        /// The date and time at which the reaction was posted.
        /// </summary>
        public DateTime DatePosted { get; set; }

        /// <summary>
        /// Indicates whether this reaction is public or private.
        /// </summary>
        public bool IsPrivate { get; set; }

        //TODO: Add metadata from media service? To reactions
        /// <summary>
        /// Metadata from the Media Service.
        /// </summary>
        public string MediaServiceData { get; set; }
    }
}