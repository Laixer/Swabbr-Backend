using Microsoft.AspNetCore.Mvc;
using Swabbr.Core.Interfaces;
using System;
using System.Threading.Tasks;

namespace Swabbr.Api.Controllers
{
    /// <summary>
    /// Actions related to follow requests.
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class FollowRequestsController : ControllerBase
    {

        // TODO Return user objects or user id's???
        /// <summary>
        /// Returns a collection of users who have a pending follow request for the authenticated user.
        /// </summary>
        [HttpGet("incoming")]
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
        public async Task<IActionResult> Outgoing()
        {
            //! TODO
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a collection of users for which the authenticated user has previously rejected follow requests.
        /// </summary>
        /// <returns></returns>
        [HttpGet("rejected")]
        public async Task<IActionResult> Rejected()
        {
            //! TODO
            throw new NotImplementedException();
        }

        /// <summary>
        /// Send a follow request to the specified user.
        /// </summary>
        [HttpPost("create")]
        public async Task<IActionResult> Create()
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
        public async Task<IActionResult> Accept()
        {
            //! TODO
            throw new NotImplementedException();
        }

        /// <summary>
        /// Decline a follow request for the authenticated user.
        /// </summary>
        [HttpPut("decline")]
        public async Task<IActionResult> Decline()
        {
            //! TODO
            throw new NotImplementedException();
        }
    }
}