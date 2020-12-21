using Swabbr.Core.Entities;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swabbr.Testing.Repositories
{
    /// <summary>
    ///     Test repository for <see cref="FollowRequest"/> entities.
    /// </summary>
    public class TestFollowRequestRepository : TestRepositoryBase<FollowRequest, FollowRequestId>, IFollowRequestRepository
    {
        public Task<FollowRequestId> CreateAsync(FollowRequest entity) => throw new NotImplementedException();
        public Task DeleteAsync(FollowRequestId id) => throw new NotImplementedException();
        public Task<bool> ExistsAsync(FollowRequestId id) => throw new NotImplementedException();
        public IAsyncEnumerable<FollowRequest> GetAllAsync(Navigation navigation) => throw new NotImplementedException();
        public Task<FollowRequest> GetAsync(FollowRequestId id) => throw new NotImplementedException();
        public Task<uint> GetFollowerCountAsync(Guid userId) => throw new NotImplementedException();
        public Task<uint> GetFollowingCountAsync(Guid userId) => throw new NotImplementedException();
        public IAsyncEnumerable<FollowRequest> GetIncomingForUserAsync(Navigation navigation) => throw new NotImplementedException();
        public IAsyncEnumerable<FollowRequest> GetOutgoingForUserAsync(Navigation navigation) => throw new NotImplementedException();
        public Task UpdateStatusAsync(FollowRequestId id, FollowRequestStatus status) => throw new NotImplementedException();
    }
}
