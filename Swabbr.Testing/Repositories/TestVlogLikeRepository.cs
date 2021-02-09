using Swabbr.Core.Entities;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swabbr.Testing.Repositories
{
    /// <summary>
    ///     Test repository for <see cref="VlogLike"/> entities.
    /// </summary>
    public class TestVlogLikeRepository : TestRepositoryBase<VlogLike, VlogLikeId>, IVlogLikeRepository
    {
        public Task<VlogLikeId> CreateAsync(VlogLike entity) => throw new NotImplementedException();
        public Task DeleteAsync(VlogLikeId id) => throw new NotImplementedException();
        public Task<bool> ExistsAsync(VlogLikeId id) => throw new NotImplementedException();
        public IAsyncEnumerable<VlogLike> GetAllAsync(Navigation navigation) => throw new NotImplementedException();
        public Task<VlogLike> GetAsync(VlogLikeId id) => throw new NotImplementedException();
        public IAsyncEnumerable<VlogLike> GetForVlogAsync(Guid vlogId, Navigation navigation) => throw new NotImplementedException();
        public Task<VlogLikeSummary> GetSummaryForVlogAsync(Guid vlogId) => throw new NotImplementedException();
        public IAsyncEnumerable<VlogLikingUserWrapper> GetVlogLikingUsersForUserAsync(Navigation navigation) => throw new NotImplementedException();
    }
}
