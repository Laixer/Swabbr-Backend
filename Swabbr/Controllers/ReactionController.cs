using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Swabbr.Api.Controllers
{
    /// <summary>
    /// Actions related to reactions to vlogs.
    /// </summary>
    [Obsolete("Not yet supported/implemented")]
    [ApiController]
    [Route("api/v1/reactions")]
    public class ReactionController : ControllerBase
    {
        /// <summary>
        /// Create a new reaction to a vlog.
        /// </summary>
        [HttpPost("create/vlogs/{vlogId}")]
        public async Task<IActionResult> Create()
        {
            //! TODO
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get reactions to a vlog.
        /// </summary>
        [HttpGet("create/vlogs/{vlogId}")]
        public async Task<IActionResult> GetAll()
        {
            //! TODO
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get a single reaction to a vlog.
        /// </summary>
        [HttpGet("{reactionId}")]
        public async Task<IActionResult> GetReaction()
        {
            //! TODO
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get reactions to the specified vlog.
        /// </summary>
        [HttpGet("vlogs/{vlogId}")]
        public async Task<IActionResult> GetReactions()
        {
            //! TODO
            throw new NotImplementedException();
        }

        /// <summary>
        /// Delete a reaction to a vlog for the authenticated user.
        /// </summary>
        [HttpDelete("{reactionId}")]
        public async Task<IActionResult> Delete()
        {
            //! TODO
            throw new NotImplementedException();
        }
    }
}