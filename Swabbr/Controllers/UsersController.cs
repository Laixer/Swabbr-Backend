using Microsoft.AspNetCore.Mvc;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Interfaces;
using Swabbr.Core.Models;
using System;
using System.Threading.Tasks;

namespace Swabbr.Controllers
{
    //TODO Determine attributes
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _repo;

        public UsersController(IUserRepository repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Create a new user account.
        /// </summary>
        /// <param name="user">New user information</param>
        /// <returns></returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserItem user)
        {
            var y = await _repo.AddAsync(user);

            //! TODO
            return Ok(y);
        }

        /// <summary>
        /// Authorizes a registered user.
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserItem user)
        {
            //! TODO
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deauthorizes the authenticated user.
        /// </summary>
        [HttpDelete("logout")]
        public IActionResult Logout()
        {
            // TODO Deauthorize user, delete user access token
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get information about a single user.
        /// </summary>
        [HttpGet("{userId}")]
        public async Task<IActionResult> Get([FromRoute]string userId)
        {
            try
            {
                var user = await _repo.GetByIdAsync(userId);
                return new OkObjectResult(user);
            }
            catch (EntityNotFoundException)
            {
                return new NotFoundResult();
            }
        }

        /// <summary>
        /// Search for users.
        /// </summary>
        /// <param name="q">Search query.</param>
        /// <param name="offset">To be used for pagination.</param>
        /// <param name="limit">Maximum amount of results to fetch.</param>
        [HttpGet("search")]
        public async Task<IActionResult> Search(
            [FromQuery]string q,
            [FromQuery]uint offset = 0,
            [FromQuery]uint limit = 1)
        {
            var results = await _repo.SearchAsync(q, offset, limit);

            return Ok(results);

            //! TODO
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get information about the authenticated user.
        /// </summary>
        [HttpGet("self")]
        public async Task<IActionResult> Self()
        {
            //! TODO
            throw new NotImplementedException();
        }

        /// <summary>
        /// Update the authenticated user.
        /// </summary>
        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody] UserItem user)
        {
            try
            {
                await _repo.UpdateAsync(user);
                return Ok(user);
            }
            catch
            {
                return new BadRequestResult();
            }

            //! TODO
            throw new NotImplementedException();
        }

        //TODO Should these contain a reference to the requirements?
        /// <summary>
        /// Delete the account of the authenticated user.
        /// </summary>
     // TODO To remove   
            ////[ApiExplorerSettings(IgnoreApi = true)]
        [HttpDelete("delete")]
        public async Task<IActionResult> Delete()
        {
            UserItem user = null;
            await _repo.DeleteAsync(user);
            //! TODO
            throw new NotImplementedException();
        }
    }
}