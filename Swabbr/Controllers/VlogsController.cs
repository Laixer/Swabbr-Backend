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
    /// Controller for handling requests related to vlogs.
    /// </summary>
    [ApiController]
    [Route("api/v1/vlogs")]
    public class VlogsController : ControllerBase
    {
        private readonly IVlogRepository _vlogRepository;
        private readonly IVlogLikeRepository _vlogLikeRepository;
        private readonly UserManager<SwabbrIdentityUser> _userManager;

        public VlogsController(
            IVlogRepository vlogRepository, 
            IVlogLikeRepository vlogLikeRepository, 
            UserManager<SwabbrIdentityUser> userManager
            )
        {
            _vlogRepository = vlogRepository;
            _vlogLikeRepository = vlogLikeRepository;
            _userManager = userManager;
        }

        /// <summary>
        /// Get a single vlog.
        /// </summary>
        [HttpGet("{vlogId}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(VlogOutputModel))]
        public async Task<IActionResult> Get([FromRoute]Guid vlogId)
        {
            //TODO Not implemented
            return Ok(MockRepository.RandomVlogOutput());
        }

        /// <summary>
        /// Get a collection of featured vlogs.
        /// </summary>
        /// <returns></returns>
        [HttpGet("featured")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<VlogOutputModel>))]
        public async Task<IActionResult> Featured()
        {
            //TODO Not implemented
            return Ok(Enumerable.Repeat(MockRepository.RandomVlogOutput(), 10));
        }

        /// <summary>
        /// Get a collection of featured vlogs with users.
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/v2/vlogs/featured")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<VlogWithUserOutputModel>))]
        public async Task<IActionResult> FeaturedWithUsers()
        {
            var vlogsWithUsers = Enumerable.Repeat(new VlogWithUserOutputModel()
            {
                User = MockRepository.RandomUserOutputMock(Guid.NewGuid()),
                Vlog = MockRepository.RandomVlogOutput()
            }, 10);

            //TODO Not implemented
            return Ok(vlogsWithUsers);
        }

        // TODO Specify limit?
        /// <summary>
        /// Get vlogs from the specified user.
        /// </summary>
        [HttpGet("users/{userId}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<VlogOutputModel>))]
        public async Task<IActionResult> ListForUser([FromRoute]Guid userId)
        {
            //TODO Not implemented
            return Ok(Enumerable.Repeat(MockRepository.RandomVlogOutput(), 5));
        }

        /// <summary>
        /// Delete a vlog that is owned by the authenticated user.
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{vlogId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> Delete([FromRoute]Guid vlogId)
        {
            var identityUser = await _userManager.GetUserAsync(User);

            var vlogEntity = await _vlogRepository.GetByIdAsync(vlogId);

            // Ensure the authenticated user is the owner of this vlog
            if (!vlogEntity.UserId.Equals(identityUser.UserId))
            {
                return BadRequest();
            }

            await _vlogRepository.DeleteAsync(vlogEntity);

            return NoContent();
        }

        // TODO What to return? Maybe an updated model of the vlog? Or the amount of likes for the vlog.
        /// <summary>
        /// Leave a like on a single vlog.
        /// </summary>
        [HttpPost("like/{vlogId}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> Like([FromRoute]Guid vlogId)
        {
            // TODO Check if the vlog exists first?
            var identityUser = await _userManager.GetUserAsync(User);

            var entityToCreate = new VlogLike
            {
                VlogLikeId = Guid.NewGuid(),
                UserId = identityUser.UserId,
                VlogId = vlogId,
                TimeCreated = DateTime.Now,
            };

            // Create a new like record
            await _vlogLikeRepository.CreateAsync(entityToCreate);

            return Ok();
        }

        /// <summary>
        /// Remove a like previously given to a single vlog.
        /// </summary>
        [HttpDelete("like/{vlogId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> Unlike([FromRoute]Guid vlogId)
        {
            // TODO Check if the vlog exists first?
            var identityUser = await _userManager.GetUserAsync(User);

            try
            {
                // Retrieve and delete the entity
                var entityToDelete = await _vlogLikeRepository.GetByUserIdAsync(vlogId, identityUser.UserId);
                await _vlogLikeRepository.DeleteAsync(entityToDelete);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }

            return NoContent();
        }
    }
}