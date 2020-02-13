﻿using Swabbr.Core.Entities;
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
        /// Returns a single follow request entity from the id of the requester to the id of the receiver.
        /// </summary>
        /// <returns></returns>
        Task<FollowRequest> GetByUserIdAsync(Guid receiverId, Guid requesterId);

        /// <summary>
        /// Returns whether a follow relationship from the receiver to the requester exists.
        /// </summary>
        /// <param name="receiverId">Unique identifier of the receiving user.</param>
        /// <param name="requesterId">Unique identifier of the requesting user.</param>
        /// <returns></returns>
        /// TODO THOMAS I suspect that this exists to battle the double-follow-request race conditions. These should
        /// never be a problem as long as the follow request processsing pipeline is transactional. --> postgresql
        Task<bool> ExistsAsync(Guid receiverId, Guid requesterId);

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
    }
}