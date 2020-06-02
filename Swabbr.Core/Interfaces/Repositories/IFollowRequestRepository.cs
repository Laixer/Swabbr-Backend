using Swabbr.Core.Entities;
using Swabbr.Core.Enums;
using Swabbr.Core.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.Repositories
{

    /// <summary>
    /// Contract for the <see cref="FollowRequest"/> repository.
    /// </summary>
    public interface IFollowRequestRepository : IRepository<FollowRequest, FollowRequestId>, ICudFunctionality<FollowRequest, FollowRequestId>
    {

        /// <summary>
        /// Returns whether a follow relationship from the receiver to the requester exists.
        /// </summary>
        /// <param name="followRequestId"><see cref="FollowRequestId"/></param>
        /// <returns></returns>
        /// TODO THOMAS I suspect that this exists to battle the double-follow-request race conditions. These should
        /// never be a problem as long as the follow request processsing pipeline is transactional. --> postgresql
        Task<bool> ExistsAsync(FollowRequestId followRequestId);

        /// <summary>
        /// Returns all follow requests targeted to a specific user.
        /// </summary>
        /// <param name="userId">Unique identifier of the user that received the requests.</param>
        /// <returns></returns>
        Task<IEnumerable<FollowRequest>> GetIncomingForUserAsync(Guid userId);

        /// <summary>
        /// Returns all outgoing follow requests from a specific user.
        /// </summary>
        /// <param name="userId">Unique identifier of the user that sent out the requests.</param>
        /// <returns></returns>
        Task<IEnumerable<FollowRequest>> GetOutgoingForUserAsync(Guid userId);

        /// <summary>
        /// Returns the amount of users that follow the specified user.
        /// </summary>
        /// <param name="userId">Unique identifier of the user that is being followed.</param>
        /// <returns></returns>
        Task<int> GetFollowerCountAsync(Guid userId);

        /// <summary>
        /// Returns the amount of users that the specified user is following.
        /// </summary>
        /// <param name="userId">
        /// Unique identifier of the user to check the amount of followers for.
        /// </param>
        /// <returns></returns>
        Task<int> GetFollowingCountAsync(Guid userId);

        /// <summary>
        /// Updates the status for a single <see cref="FollowRequest"/> to the 
        /// specified <paramref name="status"/>.
        /// </summary>
        /// <param name="id">Internal <see cref="FollowRequest"/> id</param>
        /// <param name="status"><see cref="FollowRequestStatus"/></param>
        /// <returns><see cref="Task"/></returns>
        Task<FollowRequest> UpdateStatusAsync(FollowRequestId id, FollowRequestStatus status);

    }

}
