using Microsoft.AspNetCore.Mvc;
using Swabbr.Api.ViewModels;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Swabbr.Api.Controllers
{
    /// <summary>
    /// Actions related to users that one is following.
    /// </summary>
    [Obsolete]
    [ApiController]
    [Route("api/v1/following")]
    public class FollowingController : ControllerBase
    {
        /// <summary>
        /// Returns a collection of users that the specified user is following.
        /// </summary>
        [HttpGet("users/{userId}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<UserOutputModel>))]
        public async Task<IActionResult> List([FromRoute] int userId)
        {
            return Ok(
                new UserOutputModel[]
                {
                    UserOutputModel.NewRandomMock(),
                    UserOutputModel.NewRandomMock(),
                    UserOutputModel.NewRandomMock()
                }
            );

            //! TODO
            throw new NotImplementedException();
        }

        /// <summary>
        /// Unfollow the specified user.
        /// </summary>
        [HttpDelete("users/{userId}/unfollow")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> Unfollow([FromRoute] int userId)
        {
            return NoContent();
            //! TODO
            throw new NotImplementedException();
        }
    }
}