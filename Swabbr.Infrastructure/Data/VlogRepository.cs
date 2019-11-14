using Microsoft.Azure.Cosmos;
using Swabbr.Core.Interfaces;
using Swabbr.Core.Models;
using System;

namespace Swabbr.Infrastructure.Data
{
    public class VlogRepository : CosmosDbRepository<VlogItem>, IVlogRepository
    {
        public VlogRepository(ICosmosDbClientFactory factory) : base(factory) { }

        public override string ContainerName { get; } = "Vlogs";
        public override string GenerateId(VlogItem entity) => Guid.NewGuid().ToString();
        public override PartitionKey ResolvePartitionKey(string entityId) => new PartitionKey(entityId);
    }
}
