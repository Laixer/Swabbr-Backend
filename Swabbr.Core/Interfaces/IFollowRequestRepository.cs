using Swabbr.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces
{
    public interface IFollowRequestRepository : IRepository<FollowRequest>
    {
        Task<FollowRequest> GetByIdAsync(Guid followRequestId);

        Task<IEnumerable<FollowRequest>> GetIncomingRequestsForUserAsync(Guid userId);

        Task<IEnumerable<FollowRequest>> GetOutgoingRequestsForUserAsync(Guid userId);
    }
}
