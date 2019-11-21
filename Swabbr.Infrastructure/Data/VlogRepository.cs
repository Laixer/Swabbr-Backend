using Microsoft.Azure.Cosmos;
using Swabbr.Core.Documents;
using Swabbr.Core.Interfaces;
using System;

namespace Swabbr.Infrastructure.Data
{
    public class VlogRepository : CosmosDbRepository<VlogDocument>, IVlogRepository
    {
        public VlogRepository(ICosmosDbClientFactory factory) : base(factory) { }

        public override string ContainerName { get; } = "Vlogs";
        public override string GenerateId(VlogDocument entity) => Guid.NewGuid().ToString();
        public override PartitionKey ResolvePartitionKey(string entityId) => new PartitionKey(entityId);
    }
}
