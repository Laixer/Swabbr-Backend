using Swabbr.Core.Interfaces;
using Swabbr.Core.Models;

namespace Swabbr.Infrastructure.Data
{
    public class FollowRequestsRepository : CosmosDbRepository<FollowRequest>, IFollowRequestsRepository
    {
        private readonly ICosmosDbClientFactory _factory;

        public FollowRequestsRepository(ICosmosDbClientFactory factory) : base(factory)
        {
            _factory = factory;
        }

        public override string ContainerName => "FollowRequests";
    }
}
