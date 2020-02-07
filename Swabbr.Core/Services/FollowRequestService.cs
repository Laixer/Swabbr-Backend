using Swabbr.Core.Entities;
using Swabbr.Core.Enums;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Interfaces;
using Swabbr.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Swabbr.Core.Services
{
    public class FollowRequestService : IFollowRequestService
    {
        private readonly IFollowRequestRepository _followRequestRepository;
        private readonly IUserSettingsRepository _userSettingsRepository;

        public FollowRequestService(
            IFollowRequestRepository followRequestRepository,
            IUserSettingsRepository userSettingsRepository
            )
        {
            _followRequestRepository = followRequestRepository;
            _userSettingsRepository = userSettingsRepository;
        }

        public async Task<FollowRequest> SendAsync(Guid receiverId, Guid requesterId)
        {
            // In case a request between these users already exists...
            if (await _followRequestRepository.ExistsAsync(receiverId, requesterId))
            {
                var existingRequest = await _followRequestRepository.GetByUserIdAsync(receiverId, requesterId);
                if (existingRequest.Status == FollowRequestStatus.Declined)
                {
                    //TODO:Should we allow re-sending declined requests? Currently doing so by updating the status to pending.
                    existingRequest.Status = FollowRequestStatus.Pending;
                    var updatedEntity = await _followRequestRepository.UpdateAsync(existingRequest);
                    return updatedEntity;
                }

                // If we get here, a request that is either pending or accepted already exists between these users.
                throw new EntityAlreadyExistsException();
            }

            // Obtain the follow mode setting of the receiving user.
            var userSettings = await _userSettingsRepository.GetForUserAsync(receiverId);
            var followMode = userSettings.FollowMode;

            // Based on the the follow mode setting we assign the predetermined state of the follow request.
            FollowRequestStatus requestStatus;
            switch (followMode)
            {
                // Automatically accept the request.
                case FollowMode.AcceptAll:
                    requestStatus = FollowRequestStatus.Accepted;
                    break;
                // Automatically decline the accept.
                case FollowMode.DenyAll:
                    requestStatus = FollowRequestStatus.Declined;
                    break;
                // If the receiving users' follow mode is set to manual or unspecified, the state will be set to pending. 
                default:
                    requestStatus = FollowRequestStatus.Pending;
                    break;
            }

            var entityToCreate = new FollowRequest
            {
                FollowRequestId = Guid.NewGuid(),
                ReceiverId = receiverId,
                RequesterId = requesterId,
                Status = requestStatus,
                TimeCreated = DateTime.Now
            };

            return await _followRequestRepository.CreateAsync(entityToCreate);
        }

        public async Task<FollowRequest> AcceptAsync(Guid followRequestId)
        {
            var followRequest = await _followRequestRepository.GetByIdAsync(followRequestId);
            followRequest.Status = FollowRequestStatus.Accepted;
            return await _followRequestRepository.UpdateAsync(followRequest);
        }

        public async Task<FollowRequest> DeclineAsync(Guid followRequestId)
        {
            var followRequest = await _followRequestRepository.GetByIdAsync(followRequestId);
            followRequest.Status = FollowRequestStatus.Declined;
            return await _followRequestRepository.UpdateAsync(followRequest);
        }

        public Task<bool> ExistsAsync(Guid receiverId, Guid requesterId)
        {
            return _followRequestRepository.ExistsAsync(receiverId, requesterId);
        }

        public Task<FollowRequest> GetAsync(Guid followRequestId)
        {
            return _followRequestRepository.GetByIdAsync(followRequestId);
        }

        public Task<FollowRequest> GetAsync(Guid receiverId, Guid requesterId)
        {
            return _followRequestRepository.GetByUserIdAsync(receiverId, requesterId);
        }

        public Task<int> GetFollowerCountAsync(Guid userId)
        {
            return _followRequestRepository.GetFollowerCountAsync(userId);
        }

        public Task<int> GetFollowingCountAsync(Guid userId)
        {
            return _followRequestRepository.GetFollowingCountAsync(userId);
        }

        public async Task<IEnumerable<FollowRequest>> GetPendingIncomingForUserAsync(Guid userId)
        {
            return (await _followRequestRepository.GetIncomingForUserAsync(userId))
                .Where(entity => entity.Status == FollowRequestStatus.Pending);
        }

        public async Task<IEnumerable<FollowRequest>> GetPendingOutgoingForUserAsync(Guid userId)
        {
            return (await _followRequestRepository.GetOutgoingForUserAsync(userId))
                .Where(entity => entity.Status == FollowRequestStatus.Pending);
        }

        public async Task CancelAsync(Guid followRequestId)
        {
            var followRequest = await _followRequestRepository.GetByIdAsync(followRequestId);
            if (followRequest.Status == FollowRequestStatus.Pending)
            {
                // Delete the request
                await _followRequestRepository.DeleteAsync(followRequest);
            }
            throw new InvalidOperationException();
        }

        public async Task<bool> IsOwnedByUserAsync(Guid followRequestId, Guid userId)
        {
            var followRequest = await _followRequestRepository.GetByIdAsync(followRequestId);
            return followRequest.ReceiverId == userId;
        }

        public async Task UnfollowAsync(Guid receiverId, Guid requesterId)
        {
            var followRequest = await _followRequestRepository.GetByUserIdAsync(receiverId, requesterId);
            await _followRequestRepository.DeleteAsync(followRequest);
        }
    }
}
