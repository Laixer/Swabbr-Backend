using Swabbr.Core.Entities;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Types;
using Swabbr.Infrastructure.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swabbr.Infrastructure.Repositories
{
    internal class VlogRepository : RepositoryBase, IVlogRepository
    {
        public Task AddView(Guid vlogId) => throw new NotImplementedException();
        public Task<Guid> CreateAsync(Vlog entity) => throw new NotImplementedException();
        public Task DeleteAsync(Guid id) => throw new NotImplementedException();
        public Task<bool> ExistsAsync(Guid id) => throw new NotImplementedException();
        public IAsyncEnumerable<Vlog> GetAllAsync(Navigation navigation) => throw new NotImplementedException();
        public Task<Vlog> GetAsync(Guid id) => throw new NotImplementedException();
        public IAsyncEnumerable<Vlog> GetFeaturedVlogsAsync(Navigation navigation) => throw new NotImplementedException();
        public IAsyncEnumerable<Vlog> GetMostRecentVlogsForUserAsync(Guid userId, Navigation navigation) => throw new NotImplementedException();
        public IAsyncEnumerable<Vlog> GetVlogsFromUserAsync(Guid userId, Navigation navigation) => throw new NotImplementedException();
        public Task UpdateAsync(Vlog entity) => throw new NotImplementedException();
    }
}
