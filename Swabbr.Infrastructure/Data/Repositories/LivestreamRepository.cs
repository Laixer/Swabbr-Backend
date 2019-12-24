using Swabbr.Core.Entities;
using Swabbr.Infrastructure.Data.Entities;

namespace Swabbr.Infrastructure.Data.Repositories
{
    public class LivestreamRepository : DbRepository<Livestream, LivestreamTableEntity>
    {
        private readonly IDbClientFactory _factory;

        public LivestreamRepository(IDbClientFactory factory) : base(factory)
        {
            _factory = factory;
        }

        public override string TableName => "Livestreams";

        public override LivestreamTableEntity Map(Livestream entity)
        {
            return new LivestreamTableEntity
            {
                LivestreamId = entity.Id,
                Available = entity.Available,
                BroadcastLocation = entity.BroadcastLocation,
                CreatedAt = entity.CreatedAt,
                Name = entity.Name,
                PlaybackUrl = entity.PlaybackUrl,
                UpdatedAt = entity.UpdatedAt,
            };
        }

        public override Livestream Map(LivestreamTableEntity entity)
        {
            return new Livestream
            {
                Id = entity.LivestreamId,
                Available = entity.Available,
                BroadcastLocation = entity.BroadcastLocation,
                CreatedAt = entity.CreatedAt,
                Name = entity.Name,
                PlaybackUrl = entity.PlaybackUrl,
                UpdatedAt = entity.UpdatedAt,
            };
        }

        public override string ResolvePartitionKey(LivestreamTableEntity entity) => entity.BroadcastLocation;

        public override string ResolveRowKey(LivestreamTableEntity entity) => entity.LivestreamId;
    }
}
