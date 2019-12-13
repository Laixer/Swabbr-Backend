using Microsoft.AspNetCore.Mvc;
using Swabbr.Api.ViewModels;
using Swabbr.Core.Enums;
using Swabbr.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Swabbr.Api.Controllers
{
    /// <summary>
    /// Actions related to follow requests.
    /// </summary>
    [ApiController]
    [Route("api/v1/followrequests")]
    public class FollowRequestController : ControllerBase
    {
        private readonly IFollowRequestRepository _repository;

        public FollowRequestController(IFollowRequestRepository repository)
        {
            this._repository = repository;
        }

        // TODO Return user objects or user id's???
        /// <summary>
        /// Returns a collection of users who have a pending follow request for the authenticated user.
        /// </summary>
        [HttpGet("incoming")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<FollowRequestOutputModel>))]
        public async Task<IActionResult> Incoming()
        {
            //TODO Implement using authenticated user id
            return Ok(await _repository.GetIncomingRequestsForUserAsync(new Guid("0a102227-0821-4932-9869-906704d2a7d0")));
        }

        // TODO See todo above.
        /// <summary>
        /// Returns a collection of users that the authenticated user has requested to follow.
        /// </summary>
        [HttpGet("outgoing")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<FollowRequestOutputModel>))]
        public async Task<IActionResult> Outgoing()
        {
            //TODO Implement using authenticated user id
            return Ok(await _repository.GetOutgoingRequestsForUserAsync(new Guid("0a102227-0821-4932-9869-906704d2a7d0")));
        }

        /// <summary>
        /// Returns a single outgoing follow request from the authenticated user to the specified user.
        /// </summary>
        [HttpGet("outgoing/{receiverId}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(FollowRequestOutputModel))]
        public async Task<IActionResult> GetOutgoing([FromRoute]Guid receiverId)
        {
            //! TODO
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the status of a follow relationship from the authenticated user to the user with
        /// the specified id.
        /// </summary>
        [HttpGet("{receiverId}/status")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(FollowRequestStatus))]
        public async Task<IActionResult> Status([FromRoute]Guid receiverId)
        {
            return new OkObjectResult(FollowRequestStatus.Pending);

            //! TODO
            throw new NotImplementedException();
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
            //! TODO
            throw new NotImplementedException();
        }

        /// <summary>
        /// Send a follow request from the authenticated user to the specified user.
        /// </summary>
        [HttpPost("create")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(FollowRequestOutputModel))]
        public async Task<IActionResult> Create(FollowRequestInputModel input)
        {
            //TODO use input (requesterId is Authenticated user id)
            var createdEntity = await _repository.CreateAsync(new Core.Entities.FollowRequest
            {
                FollowRequestId = Guid.NewGuid(),
                ReceiverId = new Guid("0a102227-0821-4932-9869-906704d2a7d0"),
                RequesterId = Guid.NewGuid(),
                Status = FollowRequestStatus.Pending,
                TimeCreated = DateTime.Now
            });

            FollowRequestOutputModel output = createdEntity;

            return Created(Url.ToString(), output);
        }

        /// <summary>
        /// Cancel a follow request from the authenticated user sent to the specified user.
        /// </summary>
        [HttpDelete("{followRequestId}/destroy")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> Destroy([FromRoute]Guid followRequestId)
        {
            //! TODO
            throw new NotImplementedException();
        }

        //TODO: Update with model instead of accept/decline?
        /// <summary>
        /// Accept a pending follow request for the authenticated user.
        /// </summary>
        [HttpPut("{followRequestId}/accept")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(FollowRequestOutputModel))]
        public async Task<IActionResult> Accept([FromRoute]Guid followRequestId)
        {
            var todo = followRequestId;
            //! TODO
            throw new NotImplementedException();
        }

        /// <summary>
        /// Decline a follow request for the authenticated user.
        /// </summary>
        [HttpPut("{followRequestId}/decline")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(FollowRequestOutputModel))]
        public async Task<IActionResult> Decline([FromRoute]Guid followRequestId)
        {
            //! TODO
            var todo = followRequestId;
            await _repository.DeleteAsync(null);
            throw new NotImplementedException();
        }
    }
}