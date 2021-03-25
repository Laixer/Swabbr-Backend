using Swabbr.Core.Entities;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swabbr.Testing.Repositories
{
    /// <summary>
    ///     Test repository for <see cref="Reaction"/> entities.
    /// </summary>
    public class TestReactionRepository : TestRepositoryBase<Reaction, Guid>, IReactionRepository
    {
        public Task<Guid> CreateAsync(Reaction entity) => throw new NotImplementedException();
        public Task DeleteAsync(Guid id) => throw new NotImplementedException();
        public Task<bool> ExistsAsync(Guid id) => throw new NotImplementedException();
        public IAsyncEnumerable<Reaction> GetAllAsync(Navigation navigation) => throw new NotImplementedException();
        public Task<Reaction> GetAsync(Guid id) => throw new NotImplementedException();
        public Task<uint> GetCountForVlogAsync(Guid vlogId) => throw new NotImplementedException();
        public IAsyncEnumerable<Reaction> GetForVlogAsync(Guid vlogId, Navigation navigation) => throw new NotImplementedException();
        public Task<ReactionWrapper> GetWrapperAsync(Guid id) => throw new NotImplementedException();
        public IAsyncEnumerable<ReactionWrapper> GetWrappersForVlogAsync(Guid vlogId, Navigation navigation) => throw new NotImplementedException();
        public Task UpdateAsync(Reaction entity) => throw new NotImplementedException();
    }
}
