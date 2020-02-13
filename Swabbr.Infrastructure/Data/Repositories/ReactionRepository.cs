using Microsoft.Azure.Cosmos.Table;
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
    public class ReactionRepository : DbRepository<Reaction, ReactionTableEntity>, IReactionRepository
    {
        private readonly IDbClientFactory _factory;

        public ReactionRepository(IDbClientFactory factory) : base(factory)
        {
            _factory = factory;
        }

        public override string TableName { get; } = "Reactions";

        public async Task<bool> ExistsAsync(Guid reactionId)
        {
            var tableQuery = new TableQuery<DynamicTableEntity>().Where(
                TableQuery.GenerateFilterConditionForGuid(nameof(ReactionTableEntity.ReactionId), QueryComparisons.Equal, reactionId));

            return await GetEntityCountAsync(tableQuery) > 0;
        }

        public async Task<Reaction> GetByIdAsync(Guid reactionId)
        {
            var tableQuery = new TableQuery<ReactionTableEntity>().Where(
                TableQuery.GenerateFilterConditionForGuid(nameof(ReactionTableEntity.ReactionId), QueryComparisons.Equal, reactionId));

            var queryResults = await QueryAsync(tableQuery);

            if (!queryResults.Any())
            {
                throw new EntityNotFoundException();
            }

            return Map(queryResults.First());
        }

        public async Task<int> GetGivenReactionCountForUserAsync(Guid userId)
        {
            TableQuery<DynamicTableEntity> tableQuery = new TableQuery<DynamicTableEntity>()
            .Where(
                TableQuery.GenerateFilterCondition("UserId", QueryComparisons.Equal, userId.ToString())
            );
            return await GetEntityCountAsync(tableQuery);
        }

        public async Task<int> GetReactionCountForVlogAsync(Guid vlogId)
        {
            TableQuery<DynamicTableEntity> tableQuery = new TableQuery<DynamicTableEntity>()
            .Where(
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, vlogId.ToString())
            );
            return await GetEntityCountAsync(tableQuery);
        }

        public async Task<IEnumerable<Reaction>> GetReactionsByUserAsync(Guid userId)
        {
            // Retrieve records where the partition key matches the id of the user (owner)
            var tableQuery = new TableQuery<ReactionTableEntity>().Where(
                TableQuery.GenerateFilterCondition("UserId", QueryComparisons.Equal, userId.ToString()));

            var queryResults = await QueryAsync(tableQuery);

            if (!queryResults.Any())
            {
                throw new EntityNotFoundException();
            }

            // Return mapped entities
            return queryResults.Select(v => Map(v));
        }

        public async Task<IEnumerable<Reaction>> GetReactionsForVlogAsync(Guid vlogId)
        {
            // Retrieve records where the partition key matches the id of the user (owner)
            var tableQuery = new TableQuery<ReactionTableEntity>().Where(
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, vlogId.ToString()));

            var queryResults = await QueryAsync(tableQuery);

            if (!queryResults.Any())
            {
                throw new EntityNotFoundException();
            }

            // Return mapped entities
            return queryResults.Select(v => Map(v));
        }

        public override Reaction Map(ReactionTableEntity entity)
        {
            throw new NotImplementedException();
        }

        public override ReactionTableEntity Map(Reaction entity)
        {
            throw new NotImplementedException();
        }

        public override string ResolvePartitionKey(ReactionTableEntity entity) => entity.VlogId.ToString();

        public override string ResolveRowKey(ReactionTableEntity entity) => entity.ReactionId.ToString();
    }
}