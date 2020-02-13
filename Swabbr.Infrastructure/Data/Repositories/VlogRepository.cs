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
    public class VlogRepository : DbRepository<Vlog, VlogTableEntity>, IVlogRepository
    {
        private readonly IDbClientFactory _factory;

        public VlogRepository(IDbClientFactory factory) : base(factory)
        {
            _factory = factory;
        }

        public override string TableName { get; } = "Vlogs";

        public string VlogUserTableName => "VlogUser";

        public async Task<bool> ExistsAsync(Guid vlogId)
        {
            var tableQuery = new TableQuery<DynamicTableEntity>().Where(
                TableQuery.GenerateFilterConditionForGuid("RowKey", QueryComparisons.Equal, vlogId));

            return await GetEntityCountAsync(tableQuery) > 0;
        }

        public async Task<Vlog> GetByIdAsync(Guid vlogId)
        {
            var tableQuery = new TableQuery<VlogTableEntity>().Where(
                TableQuery.GenerateFilterConditionForGuid("VlogId", QueryComparisons.Equal, vlogId));

            var queryResults = await QueryAsync(tableQuery);

            if (!queryResults.Any())
            {
                throw new EntityNotFoundException();
            }

            return Map(queryResults.First());
        }

        public async Task ShareWithUserAsync(Guid vlogId, Guid userId)
        {
            var table = _factory.GetClient<VlogUserTableEntity>(VlogUserTableName).TableReference;

            var operation = TableOperation.Insert(new VlogUserTableEntity
            {
                VlogId = vlogId,
                UserId = userId
            });

            await table.ExecuteAsync(operation);
        }

        public async Task<IEnumerable<Guid>> GetSharedUserIdsAsync(Guid vlogId)
        {
            var table = _factory.GetClient<VlogUserTableEntity>(VlogUserTableName).TableReference;

            var tableQuery = new TableQuery<VlogUserTableEntity>().Where(
                TableQuery.GenerateFilterConditionForGuid("VlogId", QueryComparisons.Equal, vlogId));

            var queryResults = await table.ExecuteQueryAsync(tableQuery);

            return queryResults.Select(x => x.UserId);
        }

        public async Task<int> GetVlogCountForUserAsync(Guid userId)
        {
            TableQuery<DynamicTableEntity> tableQuery = new TableQuery<DynamicTableEntity>()
            .Where(
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, userId.ToString())
            );
            return await GetEntityCountAsync(tableQuery);
        }

        public async Task<IEnumerable<Vlog>> GetVlogsByUserAsync(Guid userId)
        {
            // Retrieve records where the partition key matches the id of the user (owner)
            var tableQuery = new TableQuery<VlogTableEntity>().Where(
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, userId.ToString()));

            var queryResults = await QueryAsync(tableQuery);

            // Return mapped entities
            return queryResults.Select(v => Map(v));
        }

        public async Task<IEnumerable<Vlog>> GetFeaturedVlogsAsync()
        {
            //!IMPORTANT
            //TODO: Determine which vlogs are featured (based on views, promotions etc). Currently returning ALL public vlogs.
            var tableQuery = new TableQuery<VlogTableEntity>().Where(
                TableQuery.GenerateFilterConditionForBool("IsPrivate", QueryComparisons.Equal, false));

            var queryResults = await QueryAsync(tableQuery);

            // Return mapped entities
            return queryResults.Select(v => Map(v));
        }

        public override Vlog Map(VlogTableEntity entity)
        {
            throw new NotImplementedException();
        }

        public override VlogTableEntity Map(Vlog entity)
        {
            throw new NotImplementedException();
        }

        public override string ResolvePartitionKey(VlogTableEntity entity) => entity.UserId.ToString();

        public override string ResolveRowKey(VlogTableEntity entity) => entity.VlogId.ToString();
    }
}