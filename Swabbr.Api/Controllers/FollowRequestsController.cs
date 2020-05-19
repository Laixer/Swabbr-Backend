using Laixer.Utility.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swabbr.Api.Authentication;
using Swabbr.Api.Errors;
using Swabbr.Api.Extensions;
using Swabbr.Api.Mapping;
using Swabbr.Api.ViewModels;
using Swabbr.Api.ViewModels.Enums;
using Swabbr.Api.ViewModels.FollowRequest;
using Swabbr.Core.Entities;
using Swabbr.Core.Enums;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Interfaces.Services;
using Swabbr.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Swabbr.Api.Controllers
{

    /// <summary>
    /// Controller for handling requests related to <see cref="FollowRequest"/> entities.
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
        /// <returns><see cref="FollowRequestCollectionOutputModel"/></returns>
        [HttpGet("incoming")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(FollowRequestCollectionOutputModel))]
        [ProducesResponseType((int)HttpStatusCode.Conflict, Type = typeof(ErrorMessage))]
        public async Task<IActionResult> IncomingAsync()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
                var incoming = await _followRequestService.GetPendingIncomingForUserAsync(user.Id).ConfigureAwait(false);
                return Ok(new FollowRequestCollectionOutputModel
                {
                    FollowRequests = incoming.Select(x => MapperFollowRequest.Map(x))
                }
                );
            }
            catch (Exception e)
            {
                logger.LogError("Could not get follow request status", e);
                return Conflict(this.Error(ErrorCodes.InvalidOperation, "Could not get status for follow request"));
            }
        }

        /// <summary>
        /// Returns a collection of <see cref="FollowRequest"/> models sent by the authenticated
        /// user that are pending.
        /// </summary>
        /// <returns><see cref="FollowRequestCollectionOutputModel"/></returns>
        [HttpGet("outgoing")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(FollowRequestCollectionOutputModel))]
        [ProducesResponseType((int)HttpStatusCode.Conflict, Type = typeof(ErrorMessage))]
        public async Task<IActionResult> OutgoingAsync()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
                var incoming = await _followRequestService.GetPendingOutgoingForUserAsync(user.Id).ConfigureAwait(false);
                return Ok(new FollowRequestCollectionOutputModel
                {
                    FollowRequests = incoming.Select(x => MapperFollowRequest.Map(x))
                }
                );
            }
            catch (Exception e)
            {
                logger.LogError("Could not get follow request status", e);
                return Conflict(this.Error(ErrorCodes.InvalidOperation, "Could not get status for follow request"));
            }
        }

        /// <summary>
        /// Returns the status of a single follow relationship from the authenticated user to the
        /// user with the specified id.
        /// </summary>
        /// <param name="receiverId">Internal receiver id</param>
        /// <returns><see cref="FollowRequestStatusOutputModel"/></returns>
        [HttpGet("outgoing/status")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(FollowRequestStatusOutputModel))]
        [ProducesResponseType((int)HttpStatusCode.Conflict, Type = typeof(ErrorMessage))]
        public async Task<IActionResult> GetStatusAsync(Guid receiverId)
        {
            try
            {
                receiverId.ThrowIfNullOrEmpty();

                var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
                if (user == null) { throw new InvalidOperationException("User can't be null"); }
                if (user.Id == receiverId) { return Conflict(this.Error(ErrorCodes.InvalidOperation, "Can't request follow request between user and himself")); }

                if (!await _userRepository.UserExistsAsync(receiverId).ConfigureAwait(false))
                {
                    return Conflict(this.Error(ErrorCodes.EntityNotFound, "Receiving user id does not exist"));
                }

                var id = new FollowRequestId { RequesterId = user.Id, ReceiverId = receiverId };
                return Ok(new FollowRequestStatusOutputModel
                {
                    Status = MapperEnum.Map(await _followRequestService.GetStatusAsync(id).ConfigureAwait(false)).GetEnumMemberAttribute()
                });
            }
            catch (EntityNotFoundException e)
            {
                return Ok(new FollowRequestStatusOutputModel
                {
                    Status = FollowRequestStatusModel.DoesNotExist.GetEnumMemberAttribute()
                });
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
        /// <param name="receiverId">Internal <see cref="Core.Entities.SwabbrUser"/> id</param>
        /// <returns><see cref="FollowRequestOutputModel"/></returns>
        [HttpPost("send")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(FollowRequestOutputModel))]
        [ProducesResponseType((int)HttpStatusCode.Conflict, Type = typeof(ErrorMessage))]
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
                    return Conflict(this.Error(ErrorCodes.EntityNotFound, "Specified user does not exist"));
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
        /// <param name="receiverId">Internal <see cref="Core.Entities.SwabbrUser"/> id</param>
        /// <remarks><see cref="IActionResult"/></remarks>
        [HttpPost("cancel")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Conflict, Type = typeof(ErrorMessage))]
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
            catch (Exception e)
            {
                logger.LogError("Error while canceling follow request", e);
                return Conflict(this.Error(ErrorCodes.InvalidOperation, "Could not cancel follow request"));
            }
        }

        /// <summary>
        /// Deletes the follow relationship from the authorized user to the specified user.
        /// </summary>
        /// <param name="receiverId">Internal <see cref="Core.Entities.SwabbrUser"/> id</param>
        /// <remarks><see cref="IActionResult"/></remarks>
        [HttpPost("unfollow")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Conflict, Type = typeof(ErrorMessage))]
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
                return Conflict(this.Error(ErrorCodes.EntityNotFound, "Relationship could not be found."));
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
        /// <param name="receiverId">Internal <see cref="Core.Entities.SwabbrUser"/> id</param>
        /// <remarks><see cref="IActionResult"/></remarks>
        [HttpPost("accept")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(FollowRequestOutputModel))]
        [ProducesResponseType((int)HttpStatusCode.Conflict, Type = typeof(ErrorMessage))]
        public async Task<IActionResult> AcceptAsync(Guid requesterId)
        {
            try
            {
                requesterId.ThrowIfNullOrEmpty();

                var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
                if (user == null) { throw new InvalidOperationException("User can't be null"); }

                var id = new FollowRequestId { RequesterId = requesterId, ReceiverId = user.Id };
                await _followRequestService.AcceptAsync(id).ConfigureAwait(false);
                return Ok();
            }
            catch (EntityNotFoundException)
            {
                return Conflict(this.Error(ErrorCodes.EntityNotFound, "Follow request was not found."));
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
        /// <param name="receiverId">Internal <see cref="Core.Entities.SwabbrUser"/> id</param>
        /// <remarks><see cref="FollowRequestOutputModel"/></remarks>
        [HttpPost("decline")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(FollowRequestOutputModel))]
        [ProducesResponseType((int)HttpStatusCode.Conflict, Type = typeof(ErrorMessage))]
        public async Task<IActionResult> DeclineAsync(Guid requesterId)
        {
            try
            {
                requesterId.ThrowIfNullOrEmpty();

                var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
                if (user == null) { throw new InvalidOperationException("User can't be null"); }

                var id = new FollowRequestId { RequesterId = requesterId, ReceiverId = user.Id };
                await _followRequestService.DeclineAsync(id).ConfigureAwait(false);
                return Ok();
            }
            catch (EntityNotFoundException)
            {
                return Conflict(this.Error(ErrorCodes.EntityNotFound, "Follow request was not found."));
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return Conflict(this.Error(ErrorCodes.InvalidOperation, "Could not decline follow request"));
            }
        }

    }

}
