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
    public class VlogRepository : DbRepository<Vlog, VlogTableEntity>, IVlogRepository
    {
        private readonly IDbClientFactory _factory;

        public VlogRepository(IDbClientFactory factory) : base(factory)
        {
            _factory = factory;
        }

        public override string TableName { get; } = "Vlogs";

        public async Task<Vlog> GetByIdAsync(Guid vlogId)
        {
            var table = _factory.GetClient<VlogTableEntity>(TableName).TableReference;

            var tq = new TableQuery<VlogTableEntity>().Where(
                TableQuery.GenerateFilterConditionForGuid("VlogId", QueryComparisons.Equal, vlogId));

            var queryResults = table.ExecuteQuery(tq);

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
            return await GetEntityCount(tableQuery);
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