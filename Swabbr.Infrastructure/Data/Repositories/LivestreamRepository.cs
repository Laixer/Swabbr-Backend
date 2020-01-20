using Microsoft.Azure.Cosmos.Table;
using Swabbr.Core.Entities;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Interfaces;
using Swabbr.Infrastructure.Data.Entities;
using System;
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

        public async Task<Livestream> ReserveLivestreamForUserAsync(Guid userId)
        {
            var table = _factory.GetClient<LivestreamTableEntity>(TableName).TableReference;

            // Check if there are any inactive streams that are available for usage.
            var tq = new TableQuery<LivestreamTableEntity>().Where(
                TableQuery.GenerateFilterConditionForBool(nameof(LivestreamTableEntity.IsActive), QueryComparisons.Equal, false));

            var queryResults = table.ExecuteQuery(tq);

            if (queryResults.Any())
            {
                // Store the reserved stream
                LivestreamTableEntity openStream = queryResults.First();

                // Bind current user to the given user id
                openStream.UserId = userId;

                // Set stream to active
                openStream.IsActive = true;

                // Update the stream to claim ownership for the given user and indicate it is being used.
                var reservedStream = await UpdateAsync(Map(openStream));

                return reservedStream;
            }

            // If we get here there are no inactive livestreams available in storage.
            throw new EntityNotFoundException("There are no livestreams available for usage.");
        }

        public override LivestreamTableEntity Map(Livestream entity)
        {
            return new LivestreamTableEntity
            {
                LivestreamId = entity.Id,
                UserId = entity.UserId,
                IsActive = entity.IsActive,
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
                UserId = entity.UserId,
                IsActive = entity.IsActive,
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