using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swabbr.Api.Authentication;
using Swabbr.Api.MockData;
using Swabbr.Api.ViewModels;
using Swabbr.Core.Entities;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Swabbr.Api.Controllers
{
    /// <summary>
    /// Controller for handling requests related to users.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/v1/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<SwabbrIdentityUser> _userManager;

        public UsersController(IUserRepository repository, UserManager<SwabbrIdentityUser> userManager)
        {
            _userRepository = repository;
            _userManager = userManager;
        }

        //TODO: Remove, temporary
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("deletetables")]
        public async Task<IActionResult> TempDeleteTables()
        {
            await _userRepository.TempDeleteTables();
            return Ok();
        }

        /// <summary>
        /// Get information about a single user.
        /// </summary>
        [HttpGet("{userId}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(UserOutputModel))]
        public async Task<IActionResult> Get([FromRoute]Guid userId)
        {
            try
            {
                User user = await _userRepository.GetByIdAsync(userId);
                UserOutputModel output = user;
                return Ok(output);
            }
            catch (EntityNotFoundException)
            {
                return NotFound(userId);
            }
        }

        // TODO remove unused parameters
        /// <summary>
        /// Search for users.
        /// </summary>
        /// <param name="query">Search query.</param>
        [HttpGet("search")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<UserOutputModel>))]
        public async Task<IActionResult> Search([FromQuery]string query)
        {
            var users = await _userRepository.SearchAsync(query, 0, 100);

            // Convert entities to output models
            var usersOutput = users.Select(x =>
            {
                UserOutputModel o = x;
                return o;
            });

            return Ok(usersOutput);
        }

        /// <summary>
        /// Get information about the authenticated user.
        /// </summary>
        [HttpGet("self")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(UserOutputModel))]
        public async Task<IActionResult> Self()
        {
            // TODO Remove testing code
            //var identity = User.Identity;
            //var claims = User.Claims;
            //
            //string testb = "";
            //
            //testb += (identity.IsAuthenticated ? "YES./" : "NO./");
            //testb += ($"TYPE:{identity.AuthenticationType}/");
            //testb += ($"NAME:{identity.Name}/");
            //
            //foreach (var c in claims)
            //{
            //    testb += $"{c.Value}/";
            //}
            //
            //return Ok(testb);
            var userId = Guid.Parse(User.FindFirst(SwabbrClaimTypes.UserId).Value);
            UserOutputModel output = await _userRepository.GetByIdAsync(userId);
            return Ok(output);
        }

        /// <summary>
        /// Updates the authenticated user.
        /// </summary>
        [HttpPut("update")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(UserOutputModel))]
        public async Task<IActionResult> Update([FromBody] UserUpdateInputModel input)
        {
            // TODO not implemented
            //_userRepository.UpdateAsync(input);
            return Ok(input);
        }

        /// <summary>
        /// Deletes the account of the authenticated user.
        /// </summary>
        [HttpDelete("delete")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> Delete()
        {
            // TODO Not implemented
            return NoContent();
        }

        /// <summary>
        /// Returns a collection of users that the specified user is following.
        /// </summary>
        [HttpGet("users/{userId}/following")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<UserOutputModel>))]
        public async Task<IActionResult> List([FromRoute] Guid userId)
        {
            // TODO Not implemented
            return Ok(
                Enumerable.Repeat(MockRepository.RandomUserOutputMock(), 10)
            );
        }

        /// <summary>
        /// Deletes the follow relationship from the authorized user to the specified user.
        /// </summary>
        [HttpDelete("users/{userId}/unfollow")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> Unfollow([FromRoute] Guid userId)
        {
            // TODO Not implemented
            return NoContent();
        }

        /// <summary>
        /// Get the followers of a single user.
        /// </summary>
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<UserOutputModel>))]
        [HttpGet("users/{userId}/followers")]
        public async Task<IActionResult> ListFollowers([FromRoute] Guid userId)
        {
            //TODO Not implemented
            return Ok(
                Enumerable.Repeat(MockRepository.RandomUserOutputMock(), 20)
            );
        }
    }
}