using Laixer.Utility.Extensions;
using Microsoft.Extensions.Logging;
using Swabbr.Core.Entities;
using Swabbr.Core.Enums;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Interfaces.Services;
using Swabbr.Core.Types;
using Swabbr.Core.Utility;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;

namespace Swabbr.Core.Services
{

    /// <summary>
    /// Contains functionality to handle <see cref="FollowRequest"/> operations.
    /// </summary>
    public class FollowRequestService : IFollowRequestService
    {

        private readonly IFollowRequestRepository _followRequestRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger logger;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public FollowRequestService(IFollowRequestRepository followRequestRepository,
            IUserRepository userRepository,
            ILoggerFactory loggerFactory)
        {
            _followRequestRepository = followRequestRepository ?? throw new ArgumentNullException(nameof(followRequestRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            logger = (loggerFactory != null) ? loggerFactory.CreateLogger(nameof(FollowRequestService)) : throw new ArgumentNullException(nameof(loggerFactory));
        }

        /// <summary>
        /// Sends a <see cref="FollowRequest"/>. If a previous one already exists
        /// between the sender and receiver and has been declined, the request will
        /// be sent again.
        /// </summary>
        /// <remarks>
        /// Any operations regarding user settings (being auto accept and/or 
        /// auto decline) are handled by our database.
        /// </remarks>
        /// <param name="requesterId">Requesting <see cref="SwabbrUser"/> internal id</param>
        /// <param name="receiverId">Receiving <see cref="SwabbrUser"/> internal id</param>
        /// <returns><see cref="FollowRequest"/></returns>
        public async Task<FollowRequest> SendAsync(Guid requesterId, Guid receiverId)
        {
            receiverId.ThrowIfNullOrEmpty();
            requesterId.ThrowIfNullOrEmpty();
            var id = new FollowRequestId { RequesterId = requesterId, ReceiverId = receiverId };

            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            // Handle if the request exists.
            if (await _followRequestRepository.ExistsAsync(id).ConfigureAwait(false))
            {
                // TODO This can be done with a database function maybe
                var existingRequest = await _followRequestRepository.GetAsync(id).ConfigureAwait(false);
                if (existingRequest.FollowRequestStatus == FollowRequestStatus.Declined)
                {
                    existingRequest.FollowRequestStatus = FollowRequestStatus.Pending;
                    var updatedEntity = await _followRequestRepository.UpdateStatusAsync(existingRequest.Id, FollowRequestStatus.Pending).ConfigureAwait(false);

                    // Commit and return
                    scope.Complete();
                    return updatedEntity;
                }

                // If we get here, a request that is either pending or accepted already exists between these users.
                throw new EntityAlreadyExistsException("A follow request between the requesting and receiving user already exists (and is not declined)");
            }

            // Continue if the request does not exist yet
            var result = await _followRequestRepository.CreateAsync(new FollowRequest
            {
                Id = new FollowRequestId
                {
                    ReceiverId = receiverId,
                    RequesterId = requesterId
                }
            }).ConfigureAwait(false);

            // Commit and return
            scope.Complete();
            return result;
        }

        /// <summary>
        /// Accepts a <see cref="FollowRequest"/>.
        /// </summary>
        /// <param name="id"><see cref="FollowRequestId"/></param>
        /// <returns></returns>
        public async Task<FollowRequest> AcceptAsync(FollowRequestId id)
        {
            id.ThrowIfNullOrEmpty();

            var followRequest = await _followRequestRepository.GetAsync(id).ConfigureAwait(false);
            if (followRequest.FollowRequestStatus != FollowRequestStatus.Pending) { throw new InvalidOperationException("Can't accept a non-pending follow request"); }

            return await _followRequestRepository.UpdateStatusAsync(id, FollowRequestStatus.Accepted).ConfigureAwait(false);
        }

        public async Task<FollowRequest> DeclineAsync(FollowRequestId id)
        {
            id.ThrowIfNullOrEmpty();

            var followRequest = await _followRequestRepository.GetAsync(id).ConfigureAwait(false);
            if (followRequest.FollowRequestStatus != FollowRequestStatus.Pending) { throw new InvalidOperationException("Can't accept a non-pending follow request"); }

            return await _followRequestRepository.UpdateStatusAsync(id, FollowRequestStatus.Declined).ConfigureAwait(false);
        }

        public Task<FollowRequest> GetAsync(FollowRequestId id)
        {
            return _followRequestRepository.GetAsync(id);
        }

        /// <summary>
        /// Gets the <see cref="FollowRequestStatus"/> for a <see cref="FollowRequest"/>
        /// between two <see cref="SwabbrUser"/>s.
        /// </summary>
        /// <remarks>
        /// TODO Maybe a separate call for the repository to save processing power? Minor optimization
        /// </remarks>
        /// <param name="id"><see cref="FollowRequestId"/></param>
        /// <returns><see cref="FollowRequestStatus"/></returns>
        public async Task<FollowRequestStatus> GetStatusAsync(FollowRequestId id)
        {
            if (id == null) { throw new ArgumentNullException(nameof(id)); }
            id.RequesterId.ThrowIfNullOrEmpty();
            id.ReceiverId.ThrowIfNullOrEmpty();
            return (await _followRequestRepository.GetAsync(id).ConfigureAwait(false)).FollowRequestStatus;
        }

        public Task<int> GetFollowerCountAsync(Guid userId)
        {
            return _followRequestRepository.GetFollowerCountAsync(userId);
        }

        public Task<int> GetFollowingCountAsync(Guid userId)
        {
            return _followRequestRepository.GetFollowingCountAsync(userId);
        }

        /// <summary>
        /// Lists incoming <see cref="FollowRequest"/>s.
        /// </summary>
        /// <param name="userId">Internal <see cref="SwabbrUser"/> id</param>
        /// <returns><see cref="FollowRequest"/> collection</returns>
        public Task<IEnumerable<FollowRequest>> GetPendingIncomingForUserAsync(Guid userId)
        {
            return _followRequestRepository.GetIncomingForUserAsync(userId);
        }

        /// <summary>
        /// Lists outgoing <see cref="FollowRequest"/>s.
        /// </summary>
        /// <param name="userId">Internal <see cref="SwabbrUser"/> id</param>
        /// <returns><see cref="FollowRequest"/> collection</returns>
        public Task<IEnumerable<FollowRequest>> GetPendingOutgoingForUserAsync(Guid userId)
        {
            return _followRequestRepository.GetOutgoingForUserAsync(userId);
        }

        /// <summary>
        /// Cancels a <see cref="FollowRequest"/>.
        /// </summary>
        /// <param name="id"><see cref="FollowRequestId"/></param>
        /// <returns><see cref="Task"/></returns>
        public async Task CancelAsync(FollowRequestId id)
        {
            id.ThrowIfNullOrEmpty();

            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            if (!await _followRequestRepository.ExistsAsync(id).ConfigureAwait(false))
            {
                throw new InvalidOperationException("Follow request does not exist");
            }
            // if (followRequest.Id.RequesterId != requesterId) { throw new InvalidOperationException("Follow request not owned by user"); }
            // TODO We can't really check this, you can always do this wrong.
            // We can only require the function call to have both the requester and receiver id, as we do now.

            await _followRequestRepository.DeleteAsync(id).ConfigureAwait(false);
            scope.Complete();
        }

        /// <summary>
        /// Unfollows a user.
        /// </summary>
        /// <param name="id"><see cref="FollowRequestId"/></param>
        /// <returns><see cref="Task"/></returns>
        public async Task UnfollowAsync(FollowRequestId id)
        {
            id.ThrowIfNullOrEmpty();

            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            var followRequest = await _followRequestRepository.GetAsync(id).ConfigureAwait(false);
            if (followRequest.FollowRequestStatus != FollowRequestStatus.Accepted) { throw new InvalidOperationException("Can't unfollow a non-accepted request"); }

            await _followRequestRepository.DeleteAsync(id).ConfigureAwait(false);
            scope.Complete();
        }

    }
}
