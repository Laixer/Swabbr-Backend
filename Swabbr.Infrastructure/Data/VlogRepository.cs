using Microsoft.Azure.Cosmos.Table;
using Swabbr.Core.Entities;
using Swabbr.Core.Interfaces;
using Swabbr.Infrastructure.Data.Interfaces;
using System;

namespace Swabbr.Infrastructure.Data
{
    public class VlogRepository : DbRepository<Vlog, VlogEntity>, IVlogRepository
    {
        public VlogRepository(IDbClientFactory factory) : base(factory)
        {
        }

        public override string TableName { get; } = "Vlogs";

        public override Vlog Map(VlogEntity entity)
        {
            throw new NotImplementedException();
        }

        public override VlogEntity Map(Vlog entity)
        {
            throw new NotImplementedException();
        }
    }
}