using Swabbr.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.Services
{
    public interface IFollowRequestService
    {
        /// <summary>
        /// Send a follow request from a requesting user to a receiving user.
        /// </summary>
        /// <param name="receiverId">Id of the user that should receive the request.</param>
        /// <param name="requesterId">Id of the user that sent out the request.</param>
        Task<FollowRequest> SendAsync(Guid receiverId, Guid requesterId);

        /// <summary>
        /// Accept an existing follow request.
        /// </summary>
        /// <param name="followRequestId"></param>
        /// <returns></returns>
        Task<FollowRequest> AcceptAsync(Guid followRequestId);

        /// <summary>
        /// Decline an existing follow request.
        /// </summary>
        /// <param name="followRequestId"></param>
        /// <returns></returns>
        Task<FollowRequest> DeclineAsync(Guid followRequestId);

        /// <summary>
        /// Cancel an existing follow request.
        /// </summary>
        /// <param name="followRequestId"></param>
        /// <returns></returns>
        Task CancelAsync(Guid followRequestId);

        /// <summary>
        /// Unfollow a relationship from the requester to the receiver.
        /// </summary>
        /// <param name="followRequestId"></param>
        /// <returns></returns>
        Task UnfollowAsync(Guid receiverId, Guid requesterId);

        /// <summary>
        /// Checks if an existing follow request is owned by a specific user.
        /// </summary>
        /// <param name="followRequestId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<bool> IsOwnedByUserAsync(Guid followRequestId, Guid userId);

        /// <summary>
        /// Returns a single follow request entity by id.
        /// </summary>
        /// <param name="followRequestId">Unique identifier of the follow request.</param>
        /// <returns></returns>
        Task<FollowRequest> GetAsync(Guid followRequestId);

        /// <summary>
        /// Returns a single follow request entity from the id of the requester to the id of the receiver.
        /// </summary>
        /// <returns></returns>
        Task<FollowRequest> GetAsync(Guid receiverId, Guid requesterId);

        /// <summary>
        /// Returns whether a follow relationship from the receiver to the requester exists.
        /// </summary>
        /// <param name="receiverId">Unique identifier of the receiving user.</param>
        /// <param name="requesterId">Unique identifier of the requesting user.</param>
        /// <returns></returns>
        Task<bool> ExistsAsync(Guid receiverId, Guid requesterId);

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
