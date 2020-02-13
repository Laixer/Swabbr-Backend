using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swabbr.Api.Authentication;
using Swabbr.Api.Errors;
using Swabbr.Api.Extensions;
using Swabbr.Api.ViewModels;
using Swabbr.Core.Entities;
using Swabbr.Core.Enums;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Interfaces;
using Swabbr.Core.Interfaces.Services;
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
    [Route("followrequests")]
    public class FollowRequestsController : ApiControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<SwabbrIdentityUser> _userManager;

        private readonly IFollowRequestService _followRequestService;

        public FollowRequestsController(
            IUserRepository userRepository,
            IFollowRequestService followRequestService,
            UserManager<SwabbrIdentityUser> userManager)
        {
            _userRepository = userRepository;
            _followRequestService = followRequestService;
            _userManager = userManager;
        }

        /// <summary>
        /// Returns a collection of <see cref="FollowRequest"/> models for the authenticated user
        /// that are pending.
        /// </summary>
        [HttpGet("incoming")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<FollowRequestOutputModel>))]
        public async Task<IActionResult> IncomingAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            // Retrieve all pending incoming requests for the authenticated user.
            var incomingRequests = (await _followRequestService.GetPendingIncomingForUserAsync(user.UserId))
                .Select(request => FollowRequestOutputModel.Parse(request));

            return Ok(incomingRequests);
        }

        /// <summary>
        /// Returns a collection of <see cref="FollowRequest"/> models sent by the authenticated
        /// user that are pending.
        /// </summary>
        [HttpGet("outgoing")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<FollowRequestOutputModel>))]
        public async Task<IActionResult> OutgoingAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            // Retrieve all pending outgoing requests for the authenticated user.
            var outgoingRequests = (await _followRequestService.GetPendingOutgoingForUserAsync(user.UserId))
                .Select(request => FollowRequestOutputModel.Parse(request));

            return Ok(outgoingRequests);
        }

        /// <summary>
        /// Returns a single outgoing follow request from the authenticated user to the specified user.
        /// </summary>
        [HttpGet("outgoing/{receiverId}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(FollowRequestOutputModel))]
        public async Task<IActionResult> GetSingleOutgoingAsync([FromRoute]Guid receiverId)
        {
            var identityUser = await _userManager.GetUserAsync(User);

            try
            {
                var request = await _followRequestService.GetAsync(receiverId, identityUser.UserId);
                return Ok(FollowRequestOutputModel.Parse(request));
            }
            catch (EntityNotFoundException)
            {
                return NotFound(
                    this.Error(ErrorCodes.EntityNotFound, "Outgoing request does not exist.")
                    );
            }
        }

        /// <summary>
        /// Returns the status of a single follow relationship from the authenticated user to the
        /// user with the specified id.
        /// </summary>
        [HttpGet("outgoing/{receiverId}/status")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(FollowRequestStatus))]
        //TODO: Add ProducesResponseType for all possible status codes
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ErrorMessage))]
        public async Task<IActionResult> GetStatusAsync([FromRoute]Guid receiverId)
        {
            var identityUser = await _userManager.GetUserAsync(User);

            try
            {
                var followRequest = await _followRequestService.GetAsync(receiverId, identityUser.UserId);
                return Ok(followRequest.Status);
            }
            catch (EntityNotFoundException)
            {
                return NotFound(
                    this.Error(ErrorCodes.EntityNotFound, "Outgoing request does not exist.")
                );
            }
        }

        /// <summary>
        /// Send a follow request from the authenticated user to the specified user.
        /// </summary>
        [HttpPost("send")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(FollowRequestOutputModel))]
        public async Task<IActionResult> SendAsync(Guid receiverId)
        {
            var identityUser = await _userManager.GetUserAsync(User);

            var requesterId = identityUser.UserId;

            if (requesterId == receiverId)
            {
                return Forbidden(
                    this.Error(ErrorCodes.InvalidOperation, "Users cannot follow themselves.")
                    );
            }

            //!!!
            if (!await _userRepository.UserExistsAsync(receiverId))
            {
                return NotFound(
                    this.Error(ErrorCodes.EntityNotFound, "Specified user does not exist.")
                    );
            }

            try
            {
                var followRequest = await _followRequestService.SendAsync(receiverId, requesterId);
                return Ok(FollowRequestOutputModel.Parse(followRequest));
            }
            catch (EntityAlreadyExistsException)
            {
                return Conflict(
                    this.Error(ErrorCodes.EntityAlreadyExists, "Request already exists.")
                );
            }
        }

        /// <summary>
        /// Cancel a pending follow request from the authenticated user sent to the specified user.
        /// </summary>
        [HttpDelete("{followRequestId}/cancel")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> CancelAsync([FromRoute]Guid followRequestId)
        {
            try
            {
                var identityUser = await _userManager.GetUserAsync(User);

                if (await _followRequestService.IsOwnedByUserAsync(followRequestId, identityUser.UserId))
                {
                    await _followRequestService.CancelAsync(followRequestId);
                    return NoContent();
                }

                return Forbidden(
                    this.Error(ErrorCodes.InsufficientAccessRights, "User is not allowed to modify request.")
                );
            }
            catch (InvalidOperationException)
            {
                return Conflict(
                    this.Error(ErrorCodes.InvalidOperation, "The request cannot be cancelled.")
                );
            }
            catch (EntityNotFoundException)
            {
                return NotFound(
                    this.Error(ErrorCodes.EntityNotFound, "Follow request was not found.")
                );
            }
        }

        /// <summary>
        /// Deletes the follow relationship from the authorized user to the specified user.
        /// </summary>
        [HttpDelete("unfollow/{receiverId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> UnfollowAsync([FromRoute] Guid receiverId)
        {
            var identityUser = await _userManager.GetUserAsync(User);

            try
            {
                var followRequest = await _followRequestService.GetAsync(receiverId, identityUser.UserId);
                // Delete the request
                await _followRequestService.UnfollowAsync(receiverId, identityUser.UserId);
                return NoContent();
            }
            catch (EntityNotFoundException)
            {
                return NotFound(
                    this.Error(ErrorCodes.EntityNotFound, "Relationship could not be found.")
                    );
            }
        }

        /// <summary>
        /// Accept a pending follow request for the authenticated user.
        /// </summary>
        [HttpPut("{followRequestId}/accept")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(FollowRequestOutputModel))]
        public async Task<IActionResult> AcceptAsync([FromRoute]Guid followRequestId)
        {
            try
            {
                var identityUser = await _userManager.GetUserAsync(User);

                // Ensure the authenticated user is the receiver of this follow request.
                if (await _followRequestService.IsOwnedByUserAsync(followRequestId, identityUser.UserId))
                {
                    // Accept the request.
                    var acceptedRequest = await _followRequestService.AcceptAsync(followRequestId);
                    FollowRequestOutputModel output = FollowRequestOutputModel.Parse(acceptedRequest);
                    return Ok(output);
                }

                // Deny access if the user is not the receiver of the request.
                return Forbidden();
            }
            catch (EntityNotFoundException)
            {
                return NotFound(
                    this.Error(ErrorCodes.EntityNotFound, "Follow request was not found.")
                    );
            }
        }

        /// <summary>
        /// Decline a follow request for the authenticated user.
        /// </summary>
        [HttpPut("{followRequestId}/decline")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(FollowRequestOutputModel))]
        public async Task<IActionResult> DeclineAsync([FromRoute]Guid followRequestId)
        {
            try
            {
                var identityUser = await _userManager.GetUserAsync(User);

                // Ensure the authenticated user is the receiver of this follow request.
                if (await _followRequestService.IsOwnedByUserAsync(followRequestId, identityUser.UserId))
                {
                    // Decline the request.
                    var declinedRequest = await _followRequestService.DeclineAsync(followRequestId);
                    FollowRequestOutputModel output = FollowRequestOutputModel.Parse(declinedRequest);
                    return Ok(output);
                }

                // Deny access if the user is not the receiver of the request.
                return Forbidden();
            }
            catch (EntityNotFoundException)
            {
                return NotFound(
                    this.Error(ErrorCodes.EntityNotFound, "Follow request was not found.")
                    );
            }
        }
    }
}