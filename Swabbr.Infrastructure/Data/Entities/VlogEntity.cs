using Microsoft.Azure.Cosmos.Table;
using System;

namespace Swabbr.Infrastructure.Data.Entities
{
    public class VlogEntity : TableEntity
    {
        public VlogEntity(string userId, string vlogId)
        {
            PartitionKey = userId;
            RowKey = vlogId;
        }

        /// <summary>
        /// Entity Id.
        /// </summary>
        public string VlogId { get; set; }

        /// <summary>
        /// Id of the user who created the vlog.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Indicates if the vlog should be publicly available to other users.
        /// </summary>
        public bool IsPrivate { get; set; }

        /// <summary>
        /// Indicates whether the vlog is currently live or not.
        /// </summary>
        public bool IsLive { get; set; }

        /// <summary>
        /// The date at which the recording of the vlog started.
        /// </summary>
        public DateTime DateStarted { get; set; }

        // TODO: Add Metadata from Media Service to model?
        /// <summary>
        /// Metadata from the Media Service.
        /// </summary>
        public string MediaServiceData { get; set; }
    }
}
