using System;

namespace Swabbr.Core.Entities
{
    public class Livestream : EntityBase
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string BroadcastLocation { get; set; }

        public bool Available { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }
    }
}
