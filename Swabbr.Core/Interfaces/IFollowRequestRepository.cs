using Swabbr.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces
{
    public interface IFollowRequestRepository : IRepository<FollowRequest>
    {
        /// <summary>
        /// Returns a single follow request entity by id.
        /// </summary>
        /// <param name="followRequestId">Unique identifier of the follow request.</param>
        /// <returns></returns>
        Task<FollowRequest> GetByIdAsync(Guid followRequestId);

        /// <summary>
        /// Returns the incoming (pending) follow requests for a specific user.
        /// </summary>
        /// <param name="userId">Unique identifier of the user that received the requests.</param>
        /// <returns></returns>
        Task<IEnumerable<FollowRequest>> GetIncomingForUserAsync(Guid userId);

        /// <summary>
        /// Returns the outgoing follow requests for a specific user.
        /// </summary>
        /// <param name="userId">Unique identifier of the user that sent out the requests.</param>
        /// <returns></returns>
        Task<IEnumerable<FollowRequest>> GetOutgoingForUserAsync(Guid userId);
    }
}