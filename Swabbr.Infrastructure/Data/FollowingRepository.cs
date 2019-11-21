using Swabbr.Core.Interfaces;
using Swabbr.Core.Models;

namespace Swabbr.Infrastructure.Data
{
    public class FollowingRepository : CosmosDbRepository<FollowRequest>, IFollowersRepository
    {
        private readonly ICosmosDbClientFactory _factory;

        public FollowingRepository(ICosmosDbClientFactory factory) : base(factory)
        {
            _factory = factory;
        }

        public override string ContainerName => "Following";
    }
}
