using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swabbr.Api.Authentication;
using Swabbr.Api.MockData;
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
        private readonly UserManager<SwabbrIdentityUser> _userManager;

        public FollowRequestsController(
            IFollowRequestRepository repository,
            UserManager<SwabbrIdentityUser> userManager)
        {
            _followRequestRepository = repository;
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
            return Ok(await _followRequestRepository.GetIncomingForUserAsync(user.UserId));
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
            return Ok(await _followRequestRepository.GetOutgoingForUserAsync(user.UserId));
        }

        /// <summary>
        /// Returns a single outgoing follow request from the authenticated user to the specified user.
        /// </summary>
        [HttpGet("outgoing/{receiverId}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(FollowRequestOutputModel))]
        public async Task<IActionResult> GetOutgoing([FromRoute]Guid receiverId)
        {
            //TODO Not implemented
            return Ok(Enumerable.Repeat(MockRepository.RandomUserOutputMock(), 5));
        }

        /// <summary>
        /// Returns the status of a follow relationship from the authenticated user to the user with
        /// the specified id.
        /// </summary>
        [Obsolete("May be removed. See endpoint above for followrequest status object.")]
        [HttpGet("outgoing/{receiverId}/status")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(FollowRequestStatus))]
        public async Task<IActionResult> Status([FromRoute]Guid receiverId)
        {
            //TODO Not implemented
            return Ok(FollowRequestStatus.Pending);
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
            //TODO Not implemented
            return Ok(Enumerable.Repeat(MockRepository.RandomUserOutputMock(), 5));
        }

        /// <summary>
        /// Send a follow request from the authenticated user to the specified user.
        /// </summary>
        [HttpPost("create")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(FollowRequestOutputModel))]
        public async Task<IActionResult> Create(FollowRequestInputModel input)
        {
            var identityUser = await _userManager.GetUserAsync(User);

            var entityToCreate = new FollowRequest
            {
                RequesterId = identityUser.UserId,
                Status = FollowRequestStatus.Pending,
                TimeCreated = DateTime.Now
            };

            var createdEntity = await _followRequestRepository.CreateAsync(entityToCreate);

            FollowRequestOutputModel output = createdEntity;

            return Ok(output);
        }

        /// <summary>
        /// Cancel a follow request from the authenticated user sent to the specified user.
        /// </summary>
        [HttpDelete("{followRequestId}/destroy")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> Destroy([FromRoute]Guid followRequestId)
        {
            //TODO Not implemented
            return NoContent();
        }

        //TODO: Update with model instead of accept/decline?
        /// <summary>
        /// Accept a pending follow request for the authenticated user.
        /// </summary>
        [HttpPut("{followRequestId}/accept")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(FollowRequestOutputModel))]
        public async Task<IActionResult> Accept([FromRoute]Guid followRequestId)
        {
            //TODO Not implemented
            return Ok(MockRepository.RandomFollowRequestOutput());
        }

        /// <summary>
        /// Decline a follow request for the authenticated user.
        /// </summary>
        [HttpPut("{followRequestId}/decline")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(FollowRequestOutputModel))]
        public async Task<IActionResult> Decline([FromRoute]Guid followRequestId)
        {
            //TODO Not implemented
            return Ok(MockRepository.RandomFollowRequestOutput());
        }
    }
}