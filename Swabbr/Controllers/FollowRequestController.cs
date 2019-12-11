using Microsoft.AspNetCore.Mvc;
using Swabbr.Api.ViewModels;
using Swabbr.Core.Enums;
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
        // TODO Return user objects or user id's???
        /// <summary>
        /// Returns a collection of users who have a pending follow request for the authenticated user.
        /// </summary>
        [HttpGet("incoming")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<FollowRequestOutput>))]
        public async Task<IActionResult> Incoming()
        {
            //! TODO
            throw new NotImplementedException();
        }

        // TODO See todo above.
        /// <summary>
        /// Returns a collection of users that the authenticated user has requested to follow.
        /// </summary>
        [HttpGet("outgoing")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<FollowRequestOutput>))]
        public async Task<IActionResult> Outgoing()
        {
            //! TODO
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the status of a follow relationship from the authenticated user to the user with
        /// the specified id.
        /// </summary>
        [HttpGet("status")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(FollowRequestStatus))]
        public async Task<IActionResult> Status(Guid receiverId)
        {
            return new OkObjectResult(FollowRequestStatus.Declined);

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
        /// Send a follow request to the specified user.
        /// </summary>
        [HttpPost("create")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(FollowRequestOutput))]
        public async Task<IActionResult> Create(FollowRequestInput input)
        {
            ////_repo.AddAsync(new Core.Models.FollowRequest
            ////{
            ////    ReceiverId = "test",
            ////    RequesterId = "test2",
            ////    TimeCreated = DateTime.Now
            ////});
            //! TODO
            throw new NotImplementedException();
        }

        /// <summary>
        /// Cancel a follow request sent to the specified user.
        /// </summary>
        [HttpDelete("destroy")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> Destroy()
        {
            //! TODO
            throw new NotImplementedException();
        }

        //TODO: Update with model instead of accept/decline?
        /// <summary>
        /// Accept a pending follow request for the authenticated user.
        /// </summary>
        [HttpPut("accept")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> Accept()
        {
            //! TODO
            throw new NotImplementedException();
        }

        /// <summary>
        /// Decline a follow request for the authenticated user.
        /// </summary>
        [HttpPut("decline")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> Decline()
        {
            //! TODO
            throw new NotImplementedException();
        }
    }
}