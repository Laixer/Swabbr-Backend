using Swabbr.Core.Entities;
using Swabbr.Core.Interfaces;
using Swabbr.Infrastructure.Data.Entities;
using System;

namespace Swabbr.Infrastructure.Data
{
    public class VlogRepository : DbRepository<Vlog, VlogTableEntity>, IVlogRepository
    {
        public VlogRepository(IDbClientFactory factory) : base(factory)
        {
        }

        public override string TableName { get; } = "Vlogs";

        public override Vlog Map(VlogTableEntity entity)
        {
            //TODO Implement method
            throw new NotImplementedException();
        }

        public override VlogTableEntity Map(Vlog entity)
        {
            //TODO Implement method
            throw new NotImplementedException();
        }

        public override string ResolvePartitionKey(VlogTableEntity entity) => entity.UserId.ToString();

        public override string ResolveRowKey(VlogTableEntity entity) => entity.VlogId.ToString();
    }
}