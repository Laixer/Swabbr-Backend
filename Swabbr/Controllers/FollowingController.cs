using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Swabbr.Controllers
{
    /// <summary>
    /// Actions related to users that one is following.
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class FollowingController : ControllerBase
    {
        /// <summary>
        /// Returns a collection of users that the specified user is following.
        /// </summary>
        [HttpGet("users/{userId}")]
        public async Task<IActionResult> List([FromRoute] int userId)
        {
            //! TODO
            throw new NotImplementedException();
        }
    }
}