using Swabbr.Core.Entities;
using Swabbr.Core.Interfaces;
using Swabbr.Infrastructure.Data.Entities;
using System;

namespace Swabbr.Infrastructure.Data
{
    public class RoleRepository : DbRepository<Role, RoleTableEntity>, IRoleRepository
    {
        public RoleRepository(IDbClientFactory factory) : base(factory)
        {
        }

        public override string TableName { get; } = "Roles";

        public override Role Map(RoleTableEntity entity)
        {
            //TODO Implement method
            throw new NotImplementedException();
        }

        public override RoleTableEntity Map(Role entity)
        {
            //TODO Implement method
            throw new NotImplementedException();
        }

        public override string ResolvePartitionKey(RoleTableEntity entity) => entity.RoleId.ToString();

        public override string ResolveRowKey(RoleTableEntity entity) => entity.RoleId.ToString();
    }
}
