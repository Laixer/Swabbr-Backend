using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Swabbr.Core.Documents;
using Swabbr.Core.Models;
using Swabbr.Core.Interfaces;

namespace Swabbr.Controllers
{
    /// <summary>
    /// Controller for handling vlog related Api requests.
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class VlogsController : ControllerBase
    {
        private readonly IVlogRepository _repo;

        public VlogsController(IVlogRepository repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Get a single vlog.
        /// </summary>
        [HttpGet("{vlogId}")]
        public async Task<IActionResult> Get([FromRoute]int vlogId)
        {
            //! TODO
            throw new NotImplementedException();
        }

        // TODO What is to be expected for 'featured' vlogs? (See requirements)
        /// <summary>
        /// Get a collection of featured vlogs.
        /// </summary>
        /// <returns></returns>
        [HttpGet("featured")]
        public async Task<IActionResult> Featured()
        {
            //! TODO
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get vlogs from the specified user.
        /// </summary>
        [HttpGet("users/{userId}")]
        public async Task<IActionResult> ListForUser([FromRoute]int userId)
        {
            //! TODO
            throw new NotImplementedException();
        }

        // TODO: How to handle Livestreaming Functions?
        /// <summary>
        /// Create a new vlog
        /// </summary>
        /// <param name="users">Users to share the vlog with.</param>
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody]User[] users)
        {
            //! TODO
            var x = await _repo.AddAsync(new VlogDocument
            {
                UserId = Guid.NewGuid(),
                DateStarted = DateTime.Now
            });

            return Ok(x);

            throw new NotImplementedException();
        }

        /// <summary>
        /// Delete a vlog.
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{vlogId}")]
        public async Task<IActionResult> Delete()
        {
            //! TODO
            throw new NotImplementedException();
        }

        /// <summary>
        /// Leave a like on a single vlog.
        /// </summary>
        [HttpPost("like/{vlogId}")]
        public async Task<IActionResult> Like([FromRoute]int vlogId)
        {
            //! TODO
            return new OkResult();
        }

        /// <summary>
        /// Remove a like previously given to a single vlog.
        /// </summary>
        [HttpDelete("like/{vlogId}")]
        public async Task<IActionResult> Unlike([FromRoute]int vlogId)
        {
            //! TODO
            return new OkResult();
        }
    }
}