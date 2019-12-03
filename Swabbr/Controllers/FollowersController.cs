using Microsoft.AspNetCore.Mvc;
using Swabbr.Core.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Swabbr.Api.Controllers
{
    /// <summary>
    /// Actions  related to followers of a user.
    /// </summary>
    [ApiExplorerSettings(IgnoreApi = true)]
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