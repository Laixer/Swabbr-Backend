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

        public async Task<bool> ExistsAsync(Guid vlogId)
        {
            var tq = new TableQuery<DynamicTableEntity>().Where(
                TableQuery.GenerateFilterConditionForGuid("RowKey", QueryComparisons.Equal, vlogId));

            return await GetEntityCountAsync(tq) > 0;
        }

        public async Task<Vlog> GetByIdAsync(Guid vlogId)
        {
            var table = _factory.GetClient<VlogTableEntity>(TableName).TableReference;

            var tq = new TableQuery<VlogTableEntity>().Where(
                TableQuery.GenerateFilterConditionForGuid("VlogId", QueryComparisons.Equal, vlogId));

            var queryResults = await table.ExecuteQueryAsync(tq);

            if (!queryResults.Any())
            {
                throw new EntityNotFoundException();
            }

            return Map(queryResults.First());
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
            var table = _factory.GetClient<VlogTableEntity>(TableName).TableReference;

            // Retrieve records where the partition key matches the id of the user (owner)
            var tq = new TableQuery<VlogTableEntity>().Where(
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, userId.ToString()));

            var queryResults = await table.ExecuteQueryAsync(tq);

            // Return mapped entities
            return queryResults.Select(v => Map(v));
        }

        public override Vlog Map(VlogTableEntity entity)
        {
            return new Vlog
            {
                DateStarted = entity.DateStarted,
                IsLive = entity.IsLive,
                IsPrivate = entity.IsPrivate,
                MediaServiceData = entity.MediaServiceData,
                UserId = entity.UserId,
                VlogId = entity.VlogId
            };
        }

        public override VlogTableEntity Map(Vlog entity)
        {
            return new VlogTableEntity
            {
                DateStarted = entity.DateStarted,
                IsLive = entity.IsLive,
                IsPrivate = entity.IsPrivate,
                MediaServiceData = entity.MediaServiceData,
                UserId = entity.UserId,
                VlogId = entity.VlogId
            };
        }

        public override string ResolvePartitionKey(VlogTableEntity entity) => entity.UserId.ToString();

        public override string ResolveRowKey(VlogTableEntity entity) => entity.VlogId.ToString();
    }
}