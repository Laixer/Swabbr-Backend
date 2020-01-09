using Microsoft.Azure.Cosmos.Table;
using Swabbr.Core.Entities;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Interfaces;
using Swabbr.Infrastructure.Data.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace Swabbr.Infrastructure.Data.Repositories
{
    public class LivestreamRepository : DbRepository<Livestream, LivestreamTableEntity>, ILivestreamRepository
    {
        private readonly IDbClientFactory _factory;

        public LivestreamRepository(IDbClientFactory factory) : base(factory)
        {
            _factory = factory;
        }

        public override string TableName => "Livestreams";

        public Task<Livestream> GetAvailableLivestream()
        {
            var table = _factory.GetClient<LivestreamTableEntity>(TableName).CloudTableReference;

            var tq = new TableQuery<LivestreamTableEntity>().Where(
                TableQuery.GenerateFilterConditionForBool("Available", QueryComparisons.Equal, true));

            var queryResults = table.ExecuteQuery(tq);

            if (queryResults.Any())
            {
                return Task.FromResult(Map(queryResults.First()));
            }

            throw new EntityNotFoundException();
        }

        public override LivestreamTableEntity Map(Livestream entity)
        {
            return new LivestreamTableEntity
            {
                LivestreamId = entity.Id,
                Available = entity.Available,
                BroadcastLocation = entity.BroadcastLocation,
                CreatedAt = entity.CreatedAt,
                Name = entity.Name,
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
                UpdatedAt = entity.UpdatedAt,
            };
        }

        public override string ResolvePartitionKey(LivestreamTableEntity entity) => entity.BroadcastLocation;

        public override string ResolveRowKey(LivestreamTableEntity entity) => entity.LivestreamId;
    }
}