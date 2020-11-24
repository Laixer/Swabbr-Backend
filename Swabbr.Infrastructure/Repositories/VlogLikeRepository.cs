using Swabbr.Core.Entities;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Types;
using Swabbr.Infrastructure.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swabbr.Infrastructure.Repositories
{
    internal class VlogLikeRepository : RepositoryBase, IVlogLikeRepository
    {
        public Task<VlogLikeId> CreateAsync(VlogLike entity) => throw new NotImplementedException();
        public Task DeleteAsync(VlogLikeId id) => throw new NotImplementedException();
        public Task<bool> ExistsAsync(VlogLikeId id) => throw new NotImplementedException();
        public IAsyncEnumerable<VlogLike> GetAllAsync(Navigation navigation) => throw new NotImplementedException();
        public Task<VlogLike> GetAsync(VlogLikeId id) => throw new NotImplementedException();
        public IAsyncEnumerable<VlogLike> GetForVlogAsync(Guid vlogId, Navigation navigation) => throw new NotImplementedException();
        public Task<VlogLikeSummary> GetSummaryForVlogAsync(Guid vlogId) => throw new NotImplementedException();
        public Task UpdateAsync(VlogLike entity) => throw new NotImplementedException();
    }
}
