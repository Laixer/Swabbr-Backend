﻿using Swabbr.Core.Entities;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Interfaces.Services;
using Swabbr.Core.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swabbr.Core.Services
{
    /// <summary>
    ///     Contains functionality to handle follow request operations.
    /// </summary>
    /// <remarks>
    ///     The executing user id is never passed. Whenever possible,
    ///     this id is extracted from the <see cref="AppContext"/>.
    /// </remarks>
    public class FollowRequestService : IFollowRequestService
    {
        private readonly AppContext _appContext;
        private readonly IFollowRequestRepository _followRequestRepository;

        /// <summary>
        ///     Create new instance.
        /// </summary>
        public FollowRequestService(AppContext appContext,
            IFollowRequestRepository followRequestRepository)
        {
            _appContext = appContext ?? throw new ArgumentNullException(nameof(appContext));
            _followRequestRepository = followRequestRepository ?? throw new ArgumentNullException(nameof(followRequestRepository));
        }

        /// <summary>
        ///     Accept an existing follow request.
        /// </summary>
        /// <remarks>
        ///     The receiver id is extracted from the context.
        /// </remarks>
        /// <param name="requesterId">The user that will be following.</param>
        public Task AcceptAsync(Guid requesterId)
            => _followRequestRepository.UpdateStatusAsync(
                new FollowRequestId
                {
                    RequesterId = requesterId,
                    ReceiverId = _appContext.UserId,
                },
                FollowRequestStatus.Accepted);

        /// <summary>
        ///     Cancel an existing follow request.
        /// </summary>
        /// <remarks>
        ///     The requester id is extracted from the context.
        /// </remarks>
        /// <param name="receiverId">The user that would have been followed.</param>
        public Task CancelAsync(Guid receiverId)
            => _followRequestRepository.DeleteAsync(
                new FollowRequestId
                {
                    RequesterId = _appContext.UserId,
                    ReceiverId = receiverId,
                });

        /// <summary>
        ///     Decline an existing follow request.
        /// </summary>
        /// <remarks>
        ///     The requester id is extracted from the context.
        /// </remarks>
        /// <param name="requesterId">The user that would be following.</param>
        public Task DeclineAsync(Guid requesterId)
            => _followRequestRepository.UpdateStatusAsync(
                new FollowRequestId
                {
                    RequesterId = requesterId,
                    ReceiverId = _appContext.UserId,
                },
                FollowRequestStatus.Declined);

        /// <summary>
        ///     Gets a follow request from our data store.
        /// </summary>
        /// <remarks>
        ///     If the follow request doesn't exist, a new
        ///     entity is returned with its status set to
        ///     <see cref="FollowRequestStatus.NonExistent"/>.
        ///     All other properties will not be populated.
        /// </remarks>
        /// <param name="id">The follow request id.</param>
        /// <returns>Follow request entity.</returns>
        public async Task<FollowRequest> GetAsync(FollowRequestId id)
        { 
            if (!await _followRequestRepository.ExistsAsync(id))
            {
                return new FollowRequest
                {
                    Id = id,
                    FollowRequestStatus = FollowRequestStatus.NonExistent
                };
            }

            return await _followRequestRepository.GetAsync(id);
        }

        /// <summary>
        ///     Returns the amount of users that follow the specified user.
        /// </summary>
        /// <param name="userId">Unique identifier of the user that is being followed.</param>
        /// <returns>The amount of followers.</returns>
        public Task<uint> GetFollowerCountAsync(Guid userId)
            => _followRequestRepository.GetFollowerCountAsync(userId);

        /// <summary>
        ///     Returns the amount of users that the specified user is following.
        /// </summary>
        /// <param name="userId">Id of user to check the amount of followers for.</param>
        /// <returns>User following count.</returns>
        public Task<uint> GetFollowingCountAsync(Guid userId)
            => _followRequestRepository.GetFollowingCountAsync(userId);

        /// <summary>
        ///     Returns all pending incoming follow requests for
        ///     the current user.
        /// </summary>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>Follow request collection.</returns>
        public IAsyncEnumerable<FollowRequest> GetPendingIncomingForUserAsync(Navigation navigation)
            => _followRequestRepository.GetIncomingForUserAsync(navigation);

        /// <summary>
        ///     Returns all pending outgoing follow requests
        ///     for the current user.
        /// </summary>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>Follow request collection.</returns>
        public IAsyncEnumerable<FollowRequest> GetPendingOutgoingForUserAsync(Navigation navigation)
            => _followRequestRepository.GetOutgoingForUserAsync(navigation);

        /// <summary>
        ///     Gets the status of a follow request.
        /// </summary>
        /// <param name="id">The follow request id.</param>
        /// <returns>The follow request status.</returns>
        public async Task<FollowRequestStatus> GetStatusAsync(FollowRequestId id)
        {
            var request = await GetAsync(id);

            return request.FollowRequestStatus;
        }

        /// <summary>
        ///     Send a follow request from the current user to a receiving user.
        /// </summary>
        /// <remarks>
        ///     The requester id is extracted from the context.
        /// </remarks>
        /// <param name="receiverId">User id that is being followed.</param>
        /// <returns>The created follow request id.</returns>
        public Task<FollowRequestId> SendAsync(Guid receiverId)
            => _followRequestRepository.CreateAsync(
                new FollowRequest
                {
                    Id = new FollowRequestId
                    {
                        RequesterId = _appContext.UserId,
                        ReceiverId = receiverId
                    }
                });

        /// <summary>
        ///     Unfollows the current user from a specified user.
        /// </summary>
        /// <remarks>
        ///     The requester id is extracted from the context.
        /// </remarks>
        /// <param name="receiverId">The user that will be unfollowed.</param>
        public Task UnfollowAsync(Guid receiverId)
            => _followRequestRepository.DeleteAsync(
                new FollowRequestId
                {
                    RequesterId = _appContext.UserId,
                    ReceiverId = receiverId
                });
    }
}
