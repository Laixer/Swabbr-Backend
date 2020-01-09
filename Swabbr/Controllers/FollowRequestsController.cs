using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swabbr.Api.Authentication;
using Swabbr.Api.ViewModels;
using Swabbr.Core.Entities;
using Swabbr.Core.Enums;
using Swabbr.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Swabbr.Api.Controllers
{
    /// <summary>
    /// Controller for handling requests related to follow requests.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/v1/followrequests")]
    public class FollowRequestsController : ControllerBase
    {
        private readonly IFollowRequestRepository _followRequestRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserSettingsRepository _userSettingsRepository;
        private readonly UserManager<SwabbrIdentityUser> _userManager;

        public FollowRequestsController(
            IFollowRequestRepository followRequestRepository,
            IUserRepository userRepository,
            IUserSettingsRepository userSettingsRepository,
            UserManager<SwabbrIdentityUser> userManager)
        {
            _followRequestRepository = followRequestRepository;
            _userRepository = userRepository;
            _userSettingsRepository = userSettingsRepository;
            _userManager = userManager;
        }

        /// <summary>
        /// Returns a collection of <see cref="FollowRequest"/> models for the authenticated user
        /// that are pending.
        /// </summary>
        [HttpGet("incoming")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<FollowRequestOutputModel>))]
        public async Task<IActionResult> Incoming()
        {
            var user = await _userManager.GetUserAsync(User);

            var incomingRequests = await _followRequestRepository.GetIncomingForUserAsync(user.UserId);

            // Filter out non-pending requests and map the collection to output models.
            IEnumerable<FollowRequestOutputModel> output = incomingRequests
                .Where(entity => entity.Status == FollowRequestStatus.Pending)
                .Select(entity =>
                {
                    FollowRequestOutputModel outputModel = entity;
                    return outputModel;
                });

            return Ok(output);
        }

        /// <summary>
        /// Returns a collection of <see cref="FollowRequest"/> models sent by the authenticated
        /// user that are pending.
        /// </summary>
        [HttpGet("outgoing")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<FollowRequestOutputModel>))]
        public async Task<IActionResult> Outgoing()
        {
            var user = await _userManager.GetUserAsync(User);

            var outgoingRequests = await _followRequestRepository.GetOutgoingForUserAsync(user.UserId);

            // Filter out non-pending requests and map the collection to output models.
            IEnumerable<FollowRequestOutputModel> output = outgoingRequests
                .Where(entity => entity.Status == FollowRequestStatus.Pending)
                .Select(entity =>
                {
                    FollowRequestOutputModel outputModel = entity;
                    return outputModel;
                });

            return Ok(output);
        }

        /// <summary>
        /// Returns a single outgoing follow request from the authenticated user to the specified user.
        /// </summary>
        [HttpGet("outgoing/{receiverId}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(FollowRequestOutputModel))]
        public async Task<IActionResult> GetSingleOutgoing([FromRoute]Guid receiverId)
        {
            var identityUser = await _userManager.GetUserAsync(User);
            var entity = await _followRequestRepository.GetByUserId(receiverId, identityUser.UserId);
            FollowRequestOutputModel output = entity;
            return Ok(output);
        }

        /// <summary>
        /// Returns the status of a single follow relationship from the authenticated user to the
        /// user with the specified id.
        /// </summary>
        [HttpGet("outgoing/{receiverId}/status")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(FollowRequestStatus))]
        public async Task<IActionResult> Status([FromRoute]Guid receiverId)
        {
            var identityUser = await _userManager.GetUserAsync(User);
            return Ok((await _followRequestRepository.GetByUserId(receiverId, identityUser.UserId)).Status);
        }

        /// <summary>
        /// Returns a collection of users for which the authenticated user has previously rejected
        /// follow requests.
        /// </summary>
        /// <returns></returns>
        [HttpGet("rejected")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<UserOutputModel>))]
        public async Task<IActionResult> Rejected()
        {
            var identityUser = await _userManager.GetUserAsync(User);
            var outgoingRequests = await _followRequestRepository.GetOutgoingForUserAsync(identityUser.UserId);

            // Filter out non-declined requests and map the collection to output models.
            var requestsOutput = outgoingRequests
                .Where(entity => entity.Status == FollowRequestStatus.Declined)
                .Select(entity => (FollowRequestOutputModel)entity);

            return Ok(requestsOutput);
        }

        /// <summary>
        /// Send a follow request from the authenticated user to the specified user.
        /// </summary>
        [HttpPost("send")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(FollowRequestOutputModel))]
        public async Task<IActionResult> Send(Guid receiverId)
        {
            var identityUser = await _userManager.GetUserAsync(User);

            if (identityUser.UserId.Equals(receiverId))
            {
                return BadRequest("Users cannot follow themselves.");
            }
            else if (!await _userRepository.UserExistsAsync(receiverId))
            {
                return BadRequest("User does not exist.");
            }

            // TODO Do not continue after this point if the follow request already exists? (Need a check)

            // Check follow mode setting of the receiving user.
            var userSettings = await _userSettingsRepository.GetByUserId(receiverId);
            var followMode = userSettings.FollowMode;

            // Assing the predetermined state of the follow request based on the follow mode setting.
            var requestStatus =
                (followMode == FollowMode.AcceptAll) ? FollowRequestStatus.Accepted :
                (followMode == FollowMode.DenyAll) ? FollowRequestStatus.Declined :
                FollowRequestStatus.Pending;

            var entityToCreate = new FollowRequest
            {
                FollowRequestId = Guid.NewGuid(),
                ReceiverId = receiverId,
                RequesterId = identityUser.UserId,
                Status = requestStatus,
                TimeCreated = DateTime.Now
            };

            var createdEntity = await _followRequestRepository.CreateAsync(entityToCreate);
            FollowRequestOutputModel output = createdEntity;
            return Ok(output);
        }

        /// <summary>
        /// Cancel a follow request from the authenticated user sent to the specified user.
        /// </summary>
        [HttpDelete("{followRequestId}/cancel")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> Destroy([FromRoute]Guid followRequestId)
        {
            var identityUser = await _userManager.GetUserAsync(User);
            var followRequest = await _followRequestRepository.GetByIdAsync(followRequestId);

            // Ensure the authenticated user is the requester of this follow request.
            if (identityUser.UserId.Equals(followRequest.RequesterId))
            {
                // Delete the request
                await _followRequestRepository.DeleteAsync(followRequest);
                return NoContent();
            }

            return BadRequest();
        }

        /// <summary>
        /// Accept a pending follow request for the authenticated user.
        /// </summary>
        [HttpPut("{followRequestId}/accept")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(FollowRequestOutputModel))]
        public async Task<IActionResult> Accept([FromRoute]Guid followRequestId)
        {
            var identityUser = await _userManager.GetUserAsync(User);
            var followRequest = await _followRequestRepository.GetByIdAsync(followRequestId);

            // Ensure the authenticated user is the receiver of this follow request.
            if (identityUser.UserId.Equals(followRequest.ReceiverId))
            {
                followRequest.Status = FollowRequestStatus.Accepted;
                await _followRequestRepository.UpdateAsync(followRequest);
                FollowRequestOutputModel output = followRequest;
                return Ok(output);
            }

            return BadRequest();
        }

        /// <summary>
        /// Decline a follow request for the authenticated user.
        /// </summary>
        [HttpPut("{followRequestId}/decline")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(FollowRequestOutputModel))]
        public async Task<IActionResult> Decline([FromRoute]Guid followRequestId)
        {
            var identityUser = await _userManager.GetUserAsync(User);
            var followRequest = await _followRequestRepository.GetByIdAsync(followRequestId);

            // Ensure the authenticated user is the receiver of this follow request.
            if (identityUser.UserId.Equals(followRequest.ReceiverId))
            {
                followRequest.Status = FollowRequestStatus.Declined;
                await _followRequestRepository.UpdateAsync(followRequest);
                FollowRequestOutputModel output = followRequest;
                return Ok(output);
            }

            return BadRequest();
        }
    }
}