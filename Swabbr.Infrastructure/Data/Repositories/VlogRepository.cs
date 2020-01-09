using Microsoft.Azure.Cosmos.Table;
using Swabbr.Core.Entities;
using Swabbr.Core.Interfaces;
using Swabbr.Infrastructure.Data.Entities;
using System;
using System.Threading.Tasks;

namespace Swabbr.Infrastructure.Data.Repositories
{
    public class VlogRepository : DbRepository<Vlog, VlogTableEntity>, IVlogRepository
    {
        public VlogRepository(IDbClientFactory factory) : base(factory)
        {
        }

        public override string TableName { get; } = "Vlogs";

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