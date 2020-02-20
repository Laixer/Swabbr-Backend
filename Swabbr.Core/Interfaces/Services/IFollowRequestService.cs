using Swabbr.Core.Entities;
using Swabbr.Core.Enums;
using Swabbr.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.Services
{

    /// <summary>
    /// Contains functionality for processing <see cref="FollowRequest"/> entities.
    /// </summary>
    public interface IFollowRequestService
    {

        /// <summary>
        /// Send a follow request from a requesting user to a receiving user.
        /// </summary>
        /// <param name="requesterId">Id of the user that sent out the request.</param>
        /// <param name="receiverId">Id of the user that should receive the request.</param>
        Task<FollowRequest> SendAsync(Guid requesterId, Guid receiverId);

        /// <summary>
        /// Accept an existing follow request.
        /// </summary>
        /// <param name="followRequestId"></param>
        /// <returns></returns>
        Task<FollowRequest> AcceptAsync(FollowRequestId id);

        /// <summary>
        /// Decline an existing follow request.
        /// </summary>
        /// <param name="followRequestId"></param>
        /// <returns></returns>
        Task<FollowRequest> DeclineAsync(FollowRequestId id);

        /// <summary>
        /// Cancel an existing follow request.
        /// </summary>
        /// <param name="followRequestId"></param>
        /// <param name="requesterId">The requesting <see cref="SwabbrUser"/> id</param>
        /// <returns><see cref="Task"/></returns>
        Task CancelAsync(FollowRequestId id);

        /// <summary>
        /// Unfollows a requester from a receiver.
        /// TODO Do we need functionality for the other way around? As in, kicking your subscribers?
        /// </summary>
        /// <param name="requesterId">Requesting user id</param>
        /// <param name="receiverId">Receiving user id</param>
        /// <returns><see cref="Task"/></returns>
        Task UnfollowAsync(FollowRequestId id);

        /// <summary>
        /// Returns a single follow request entity from the id of the requester to the id of the receiver.
        /// </summary>
        /// <returns></returns>
        Task<FollowRequest> GetAsync(FollowRequestId id);

        /// <summary>
        /// Gets the <see cref="FollowRequestStatus"/> for a given <see cref="FollowRequest"/>
        /// between two users.
        /// </summary>
        /// <param name="requesterId">Internal requester id</param>
        /// <param name="receiverId">Internal receiver id</param>
        /// <returns><see cref="FollowRequestStatus"/></returns>
        Task<FollowRequestStatus> GetStatusAsync(FollowRequestId id);

        /// <summary>
        /// Returns all pending incoming follow requests for a specific user.
        /// </summary>
        /// <param name="userId">Unique identifier of the user that received the requests.</param>
        /// <returns></returns>
        Task<IEnumerable<FollowRequest>> GetPendingIncomingForUserAsync(Guid userId);

        /// <summary>
        /// Returns all pending outgoing follow requests from a specific user.
        /// </summary>
        /// <param name="userId">Unique identifier of the user that sent out the requests.</param>
        /// <returns></returns>
        Task<IEnumerable<FollowRequest>> GetPendingOutgoingForUserAsync(Guid userId);

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

    }

}
