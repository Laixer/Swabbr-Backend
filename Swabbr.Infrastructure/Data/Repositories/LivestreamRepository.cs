﻿using Microsoft.Azure.Cosmos.Table;
using Swabbr.Core.Entities;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Interfaces;
using Swabbr.Infrastructure.Data.Entities;
using System;
using System.Collections.Generic;
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
            // Check if there are any inactive streams that are available for usage.
            var tableQuery = new TableQuery<LivestreamTableEntity>().Where(
                TableQuery.GenerateFilterConditionForBool(nameof(LivestreamTableEntity.IsActive), QueryComparisons.Equal, false));

            // Obtain results and order by created date so the earliest record will be used (last in
            // first out)
            var queryResults = (await QueryAsync(tableQuery)).OrderBy(x => x.CreatedAt);

            if (queryResults.Any())
            {
                // Store the reserved stream
                LivestreamTableEntity openStream = queryResults.First();

                // Bind current user to the given user id
                openStream.UserId = userId;

                // Set stream to active
                openStream.IsActive = true;

                // Update the stream to claim ownership for the given user and indicate it is being used.
                Livestream reservedStream = await UpdateAsync(Map(openStream));

                return reservedStream;
            }

            // If we get here there are no inactive livestreams available in storage.
            throw new EntityNotFoundException("There are no livestreams available for usage.");
        }

        public async Task<IEnumerable<Livestream>> GetActiveLivestreamsAsync()
        {
            // Check if there are any inactive streams that are available for usage.
            var tableQuery = new TableQuery<LivestreamTableEntity>().Where(
                TableQuery.GenerateFilterConditionForBool(nameof(LivestreamTableEntity.IsActive), QueryComparisons.Equal, true));

            var queryResults = await QueryAsync(tableQuery);

            return queryResults.Select(x => Map(x));
        }

        public async Task<Livestream> GetActiveLivestreamForUserAsync(Guid userId)
        {
            // Check if there are any inactive streams that are available for usage.
            var tableQuery = new TableQuery<LivestreamTableEntity>().Where(
                TableQuery.CombineFilters(
                    TableQuery.GenerateFilterConditionForBool(nameof(LivestreamTableEntity.IsActive), QueryComparisons.Equal, true),
                    TableOperators.And,
                    TableQuery.GenerateFilterConditionForGuid(nameof(LivestreamTableEntity.UserId), QueryComparisons.Equal, userId)
                )
            );

            var queryResults = await QueryAsync(tableQuery);

            if (queryResults.Any())
            {
                LivestreamTableEntity stream = queryResults.First();
                return Map(stream);
            }

            throw new EntityNotFoundException();
        }

        public async Task<Livestream> GetByIdAsync(string livestreamId)
        {
            var tableQuery = new TableQuery<LivestreamTableEntity>().Where(
    TableQuery.GenerateFilterCondition(nameof(LivestreamTableEntity.LivestreamId), QueryComparisons.Equal, livestreamId));

            var queryResults = await QueryAsync(tableQuery);

            if (queryResults.Any())
            {
                LivestreamTableEntity stream = queryResults.First();
                return Map(stream);
            }

            throw new EntityNotFoundException();
        }

        public async Task<int> GetAvailableLivestreamCountAsync()
        {
            TableQuery<DynamicTableEntity> tableQuery = new TableQuery<DynamicTableEntity>()
            .Where(
                TableQuery.GenerateFilterConditionForBool(nameof(LivestreamTableEntity.IsActive), QueryComparisons.Equal, false)
            );
            return await GetEntityCountAsync(tableQuery);
        }

        public override LivestreamTableEntity Map(Livestream entity)
        {
            throw new NotImplementedException();
        }

        public override Livestream Map(LivestreamTableEntity entity)
        {
            throw new NotImplementedException();
        }

        public override string ResolvePartitionKey(LivestreamTableEntity entity) => entity.BroadcastLocation;

        public override string ResolveRowKey(LivestreamTableEntity entity) => entity.LivestreamId;
    }
}