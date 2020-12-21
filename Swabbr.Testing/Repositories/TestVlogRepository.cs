using Swabbr.Core.Entities;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Swabbr.Testing.Repositories
{
    /// <summary>
    ///     Test repository for <see cref="Vlog"/> entities.
    /// </summary>
    public class TestVlogRepository : TestRepositoryBase<Vlog, Guid>, IVlogRepository
    {
        public Task AddView(Guid vlogId) => throw new NotImplementedException();
        public Task<Guid> CreateAsync(Vlog entity) => throw new NotImplementedException();
        public Task DeleteAsync(Guid id) => throw new NotImplementedException();
        public Task<bool> ExistsAsync(Guid id) => throw new NotImplementedException();
        public IAsyncEnumerable<Vlog> GetAllAsync(Navigation navigation) => throw new NotImplementedException();
        public Task<Vlog> GetAsync(Guid id) => throw new NotImplementedException();
        public IAsyncEnumerable<Vlog> GetFeaturedVlogsAsync(Navigation navigation) => throw new NotImplementedException();
        public IAsyncEnumerable<Vlog> GetMostRecentVlogsForUserAsync(Navigation navigation) => throw new NotImplementedException();
        public IAsyncEnumerable<Vlog> GetVlogsByUserAsync(Guid userId, Navigation navigation) => throw new NotImplementedException();
        public Task UpdateAsync(Vlog entity) => throw new NotImplementedException();
    }
}
