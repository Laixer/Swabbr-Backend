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
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Laixer.Utility.Extensions;
using Swabbr.Api.Mapping;
using Microsoft.Extensions.Logging;
using Swabbr.Core.Types;

namespace Swabbr.Api.Controllers
{

    /// <summary>
    /// Controller for handling requests related to <see cref="FollowRequest"/> entities.
    /// TODO Error codes more specific to our exceptions?
    /// TODO All response codes specified?
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("followrequests")]
    public class FollowRequestsController : ApiControllerBase
    {

        private readonly IUserRepository _userRepository;
        private readonly UserManager<SwabbrIdentityUser> _userManager;
        private readonly IFollowRequestService _followRequestService;
        private readonly ILogger logger;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public FollowRequestsController(IUserRepository userRepository,
            IFollowRequestService followRequestService,
            UserManager<SwabbrIdentityUser> userManager,
            ILoggerFactory loggerFactory)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _followRequestService = followRequestService ?? throw new ArgumentNullException(nameof(followRequestService));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            logger = (loggerFactory != null) ? loggerFactory.CreateLogger(nameof(FollowRequestsController)) : throw new ArgumentNullException(nameof(loggerFactory));
        }

        /// <summary>
        /// Returns a collection of <see cref="FollowRequest"/> models for the authenticated user
        /// that are pending.
        /// </summary>
        [Authorize]
        [HttpGet("incoming")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<FollowRequestOutputModel>))]
        public async Task<IActionResult> IncomingAsync()
        {
            return Conflict("Not yet implemeneted");
            //var user = await _userManager.GetUserAsync(User);

            //// Retrieve all pending incoming requests for the authenticated user.
            //var incomingRequests = (await _followRequestService.GetPendingIncomingForUserAsync(user.Id))
            //    .Select(request => FollowRequestOutputModel.Parse(request));

            //return Ok(incomingRequests);
        }

        /// <summary>
        /// Returns a collection of <see cref="FollowRequest"/> models sent by the authenticated
        /// user that are pending.
        /// </summary>
        [Authorize]
        [HttpGet("outgoing")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<FollowRequestOutputModel>))]
        public async Task<IActionResult> OutgoingAsync()
        {
            return Conflict("Not yet implemeneted");
            //var user = await _userManager.GetUserAsync(User);

            //// Retrieve all pending outgoing requests for the authenticated user.
            //var outgoingRequests = (await _followRequestService.GetPendingOutgoingForUserAsync(user.Id))
            //    .Select(request => FollowRequestOutputModel.Parse(request));

            //return Ok(outgoingRequests);
        }

        /// <summary>
        /// Returns the status of a single follow relationship from the authenticated user to the
        /// user with the specified id.
        /// </summary>
        /// <param name="receiverId">Internal receiver id</param>
        /// <returns><see cref="OkResult"/> or <see cref="ConflictResult"/></returns>
        [Authorize]
        [HttpGet("outgoing/status")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(FollowRequestStatus))]
        [ProducesResponseType((int)HttpStatusCode.Conflict, Type = typeof(ErrorMessage))]
        public async Task<IActionResult> GetStatusAsync(Guid receiverId)
        {
            try
            {
                receiverId.ThrowIfNullOrEmpty();

                var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
                if (user == null) { throw new InvalidOperationException("User can't be null"); }
                if (user.Id == receiverId) { throw new InvalidOperationException("Can't request follow request between user and itself"); }

                var id = new FollowRequestId { RequesterId = user.Id, ReceiverId = receiverId };
                return Ok(await _followRequestService.GetStatusAsync(id).ConfigureAwait(false));
            }
            catch (EntityNotFoundException e)
            {
                logger.LogError(e.Message);
                return Conflict(this.Error(ErrorCodes.EntityNotFound, "Could not find follow request"));
            }
            catch (Exception e)
            {
                logger.LogError("Could not get follow request status", e);
                return Conflict(this.Error(ErrorCodes.InvalidOperation, "Could not get status for follow request"));
            }
        }

        /// <summary>
        /// Send a follow request from the authenticated user to the specified user.
        /// </summary>
        /// <param name="receiverId">Internal receiving user id"/></param>
        /// <returns><see cref="IActionResult"/></returns>
        [Authorize]
        [HttpPost("send")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(FollowRequestOutputModel))]
        public async Task<IActionResult> SendAsync(Guid receiverId)
        {
            try
            {
                receiverId.ThrowIfNullOrEmpty();

                // We can't follow ourself
                var requesterId = (await _userManager.GetUserAsync(User).ConfigureAwait(false)).Id;
                if (requesterId == receiverId)
                {
                    return Forbidden(this.Error(ErrorCodes.InvalidOperation, "Users cannot follow themselves"));
                }

                // We can't follow a user that doesn't exist
                if (!await _userRepository.UserExistsAsync(receiverId).ConfigureAwait(false))
                {
                    return NotFound(this.Error(ErrorCodes.EntityNotFound, "Specified user does not exist"));
                }

                // Try to perform the request
                try
                {
                    var followRequest = await _followRequestService.SendAsync(requesterId, receiverId).ConfigureAwait(false);
                    return Ok(MapperFollowRequest.Map(followRequest));
                }
                catch (EntityAlreadyExistsException e)
                {
                    logger.LogError(e.Message);
                    return Conflict(this.Error(ErrorCodes.EntityAlreadyExists, "Follow request already exists (either pending or accepted)"));
                }
            }
            catch (Exception e)
            {
                logger.LogError("Error while creating a follow request", e);
                return Conflict(this.Error(ErrorCodes.InvalidOperation, "Could not create follow request"));
            }
        }

        /// <summary>
        /// Cancel a pending follow request from the authenticated user sent to the specified user.
        /// </summary>
        [Authorize]
        [HttpPost("cancel")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> CancelAsync(Guid receiverId)
        {
            try
            {
                receiverId.ThrowIfNullOrEmpty();
                var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
                if (user == null) { throw new InvalidOperationException("User can't be null"); }

                var id = new FollowRequestId { RequesterId = user.Id, ReceiverId = receiverId };
                await _followRequestService.CancelAsync(id).ConfigureAwait(false);
                return Ok();
            }
            // TODO More explicit error messages maybe
            catch (Exception e)
            {
                logger.LogError("Error while canceling follow request", e);
                return Conflict(this.Error(ErrorCodes.InvalidOperation, "Could not cancel follow request"));
            }
        }

        /// <summary>
        /// Deletes the follow relationship from the authorized user to the specified user.
        /// </summary>
        [Authorize]
        [HttpPost("unfollow")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> UnfollowAsync(Guid receiverId)
        {
            try
            {
                receiverId.ThrowIfNullOrEmpty();

                var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
                if (user == null) { throw new InvalidOperationException("User can't be null"); }

                var id = new FollowRequestId { RequesterId = user.Id, ReceiverId = receiverId };
                await _followRequestService.UnfollowAsync(id).ConfigureAwait(false);
                return Ok();
            }
            catch (EntityNotFoundException e)
            {
                logger.LogError(e.Message);
                return NotFound(this.Error(ErrorCodes.EntityNotFound, "Relationship could not be found."));
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return Conflict(this.Error(ErrorCodes.InvalidOperation, "Could not perform unfollow operation"));
            }
        }

        /// <summary>
        /// Accept a pending follow request for the authenticated user.
        /// </summary>
        [Authorize]
        [HttpPost("accept")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(FollowRequestOutputModel))]
        public async Task<IActionResult> AcceptAsync(Guid requesterId)
        {
            try
            {
                requesterId.ThrowIfNullOrEmpty();

                var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
                if (user == null) { throw new InvalidOperationException("User can't be null"); }

                var id = new FollowRequestId { RequesterId = requesterId, ReceiverId = user.Id };
                await _followRequestService.AcceptAsync(id).ConfigureAwait(false); // TODO We need the user id to check! This is error prone
                return Ok();
            }
            catch (EntityNotFoundException)
            {
                return NotFound(this.Error(ErrorCodes.EntityNotFound, "Follow request was not found."));
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return Conflict(this.Error(ErrorCodes.InvalidOperation, "Could not accept follow request"));
            }
        }

        /// <summary>
        /// Decline a follow request for the authenticated user.
        /// </summary>
        [Authorize]
        [HttpPost("decline")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(FollowRequestOutputModel))]
        public async Task<IActionResult> DeclineAsync(Guid requesterId)
        {
            try
            {
                requesterId.ThrowIfNullOrEmpty();

                var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
                if (user == null) { throw new InvalidOperationException("User can't be null"); }

                var id = new FollowRequestId { RequesterId = requesterId, ReceiverId = user.Id };
                await _followRequestService.DeclineAsync(id).ConfigureAwait(false); // TODO We need the user id to check! This is error prone
                return Ok();
            }
            catch (EntityNotFoundException)
            {
                return NotFound(this.Error(ErrorCodes.EntityNotFound, "Follow request was not found."));
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return Conflict(this.Error(ErrorCodes.InvalidOperation, "Could not decline follow request"));
            }
        }

    }

}
