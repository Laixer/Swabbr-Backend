using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Infrastructure.Abstractions;
using System;
using System.Threading.Tasks;

namespace Swabbr.Infrastructure.Repositories
{
    internal class HealthCheckRepository : RepositoryBase, IHealthCheckRepository
    {
        public Task<bool> IsAliveAsync() => throw new NotImplementedException();
    }
}
