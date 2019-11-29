using Swabbr.Core.Interfaces;
using Swabbr.Infrastructure.Data.Entities;
using Swabbr.Infrastructure.Data.Interfaces;
using System;

namespace Swabbr.Infrastructure.Data
{
    public class VlogRepository : DbRepository<VlogEntity>, IVlogRepository
    {
        public VlogRepository(IDbClientFactory factory) : base(factory) { }

        public override string TableName { get; } = "Vlogs";
       
        public override string GenerateId(VlogEntity entity) => Guid.NewGuid().ToString();

        public override string ResolveRowKey(VlogEntity entity)
        {
            return entity.VlogId.ToString();
        }

        public override string ResolvePartitionKey(VlogEntity entity)
        {
            return entity.UserId.ToString();
        }
    }
}
