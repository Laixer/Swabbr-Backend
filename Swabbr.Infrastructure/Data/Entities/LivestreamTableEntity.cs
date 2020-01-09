using Microsoft.Azure.Cosmos.Table;
using System;

namespace Swabbr.Infrastructure.Data.Entities
{
    // TODO Comments
    public class LivestreamTableEntity : TableEntity
    {
        public string LivestreamId { get; set; }

        public string Name { get; set; }

        public string BroadcastLocation { get; set; }

        public bool Available { get; set; }

        public Uri PlaybackUrl { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }
    }
}