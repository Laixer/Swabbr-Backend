using Swabbr.Core.Entities;
using Swabbr.Core.Interfaces;
using Swabbr.Infrastructure.Data.Entities;
using System;
using System.Threading.Tasks;

namespace Swabbr.Infrastructure.Data.Repositories
{
    public class VlogLikeRepository : DbRepository<VlogLike, VlogLikeTableEntity>, IVlogLikeRepository
    {
        public VlogLikeRepository(IDbClientFactory factory) : base(factory)
        {
        }

        public override string TableName { get; } = "VlogLike";

        public async Task<VlogLike> GetByUserIdAsync(Guid vlogId, Guid userId)
        {
            return await RetrieveAsync(vlogId.ToString(), userId.ToString());
        }

        public override VlogLikeTableEntity Map(VlogLike entity)
        {
            return new VlogLikeTableEntity
            {
                VlogLikeId = entity.VlogLikeId,
                VlogId = entity.VlogId,
                UserId = entity.UserId,
                TimeCreated = entity.TimeCreated
            };
        }

        public override VlogLike Map(VlogLikeTableEntity entity)
        {
            return new VlogLike
            {
                VlogLikeId = entity.VlogLikeId,
                VlogId = entity.VlogId,
                UserId = entity.UserId,
                TimeCreated = entity.TimeCreated
            };
        }

        public override string ResolvePartitionKey(VlogLikeTableEntity entity) => entity.VlogId.ToString();

        public override string ResolveRowKey(VlogLikeTableEntity entity) => entity.UserId.ToString();
    }
}