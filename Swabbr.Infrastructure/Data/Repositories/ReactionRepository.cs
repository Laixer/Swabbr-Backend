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
            //TODO: Optimize, only need to retrieve partitionkey
            try
            {
                await GetByIdAsync(reactionId);
                return true;
            }
            catch (EntityNotFoundException)
            {
                return false;
            }
        }

        public async Task<Reaction> GetByIdAsync(Guid reactionId)
        {
            var table = _factory.GetClient<VlogTableEntity>(TableName).TableReference;

            var tq = new TableQuery<ReactionTableEntity>().Where(
                TableQuery.GenerateFilterConditionForGuid("ReactionId", QueryComparisons.Equal, reactionId));

            var queryResults = await table.ExecuteQueryAsync(tq);

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
            var table = _factory.GetClient<ReactionTableEntity>(TableName).TableReference;

            // Retrieve records where the partition key matches the id of the user (owner)
            var tq = new TableQuery<ReactionTableEntity>().Where(
                TableQuery.GenerateFilterCondition("UserId", QueryComparisons.Equal, userId.ToString()));

            var queryResults = await table.ExecuteQueryAsync(tq);

            if (!queryResults.Any())
            {
                throw new EntityNotFoundException();
            }

            // Return mapped entities
            return queryResults.Select(v => Map(v));
        }

        public async Task<IEnumerable<Reaction>> GetReactionsForVlogAsync(Guid vlogId)
        {
            var table = _factory.GetClient<ReactionTableEntity>(TableName).TableReference;

            // Retrieve records where the partition key matches the id of the user (owner)
            var tq = new TableQuery<ReactionTableEntity>().Where(
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, vlogId.ToString()));

            var queryResults = await table.ExecuteQueryAsync(tq);

            if (!queryResults.Any())
            {
                throw new EntityNotFoundException();
            }

            // Return mapped entities
            return queryResults.Select(v => Map(v));
        }

        public override Reaction Map(ReactionTableEntity entity)
        {
            return new Reaction
            {
                ReactionId = entity.ReactionId,
                VlogId = entity.VlogId,
                UserId = entity.UserId,
                DatePosted = entity.DatePosted,
                IsPrivate = entity.IsPrivate,
                MediaServiceData = entity.MediaServiceData
            };
        }

        public override ReactionTableEntity Map(Reaction entity)
        {
            return new ReactionTableEntity
            {
                ReactionId = entity.ReactionId,
                VlogId = entity.VlogId,
                UserId = entity.UserId,
                DatePosted = entity.DatePosted,
                IsPrivate = entity.IsPrivate,
                MediaServiceData = entity.MediaServiceData
            };
        }

        public override string ResolvePartitionKey(ReactionTableEntity entity) => entity.VlogId.ToString();

        public override string ResolveRowKey(ReactionTableEntity entity) => entity.ReactionId.ToString();
    }
}