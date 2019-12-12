using Microsoft.AspNetCore.Mvc;
using Swabbr.Api.ViewModels;
using Swabbr.Core.Entities;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Swabbr.Api.Controllers
{
    /// <summary>
    /// Actions related to followers of a user.
    /// </summary>
    [ApiController]
    [Route("api/v1/followers")]
    public class FollowersController : ControllerBase
    {
        /// <summary>
        /// Get the followers of a single user.
        /// </summary>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<UserOutputModel>))]
        [HttpGet("users/{userId}")]
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
    }
}