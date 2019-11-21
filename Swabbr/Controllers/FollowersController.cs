using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Swabbr.Core.Models;

namespace Swabbr.Controllers
{
    /// <summary>
    /// Actions  related to followers of a user.
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class FollowersController : ControllerBase
    {
        /// <summary>Get the followers of a single user.</summary>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<User>))]
        [HttpGet("users/{userId}")]
        public async Task<IActionResult> List([FromRoute] int userId)
        {
            //! TODO
            throw new NotImplementedException();
        }
    }
}