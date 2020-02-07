using Microsoft.Azure.Cosmos.Table;
using System;

namespace Swabbr.Infrastructure.Data.Entities
{
    /// <summary>
    /// Represents the storage data for a single vlog.
    /// </summary>
    public class VlogUserTableEntity : TableEntity
    {
        /// <summary>
        /// Unique identifier of the vlog to be shared.
        /// </summary>
        public Guid VlogId { get; set; }

        /// <summary>
        /// Unique identifier of the user that should have access to the vlog.
        /// </summary>
        public Guid UserId { get; set; }
    }
}
