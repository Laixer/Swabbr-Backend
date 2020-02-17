//using Microsoft.Azure.Cosmos.Table;
//using Swabbr.Core.Entities;
//using Swabbr.Core.Interfaces;
//using Swabbr.Infrastructure.Data.Entities;
//using System;
//using System.Threading.Tasks;

//namespace Swabbr.Infrastructure.Data.Repositories
//{
//    public class VlogLikeRepository : DbRepository<VlogLike, VlogLikeTableEntity>, IVlogLikeRepository
//    {
//        public VlogLikeRepository(IDbClientFactory factory) : base(factory)
//        {
//        }

//        public override string TableName { get; } = "VlogLike";

//        public async Task<int> GetGivenCountForUserAsync(Guid userId)
//        {
//            TableQuery<DynamicTableEntity> tableQuery = new TableQuery<DynamicTableEntity>()
//            .Where(
//                TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, userId.ToString())
//            );
//            return await GetEntityCountAsync(tableQuery);
//        }

//        public async Task<VlogLike> GetSingleForUserAsync(Guid vlogId, Guid userId)
//        {
//            return await RetrieveAsync(vlogId.ToString(), userId.ToString());
//        }

//        public override VlogLikeTableEntity Map(VlogLike entity)
//        {
//            throw new NotImplementedException();
//        }

//        public override VlogLike Map(VlogLikeTableEntity entity)
//        {
//            throw new NotImplementedException();
//        }

//        public override string ResolvePartitionKey(VlogLikeTableEntity entity) => entity.VlogId.ToString();

//        public override string ResolveRowKey(VlogLikeTableEntity entity) => entity.UserId.ToString();
//    }
//}