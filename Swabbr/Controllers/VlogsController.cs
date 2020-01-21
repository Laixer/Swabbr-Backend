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
    /// Controller for handling requests related to vlogs.
    /// </summary>
    [Authorize]
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
        public async Task<IActionResult> GetAsync([FromRoute]Guid vlogId)
        {
            try
            {
                VlogOutputModel output = await _vlogRepository.GetByIdAsync(vlogId);
                return Ok(output);
            }
            catch(EntityNotFoundException)
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Get a collection of featured vlogs.
        /// </summary>
        /// <returns></returns>
        [HttpGet("featured")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<VlogOutputModel>))]
        public async Task<IActionResult> FeaturedAsync()
        {
            //TODO Not implemented
            return Ok(Enumerable.Repeat(MockRepository.RandomVlogOutput(), 10));
        }

        // TODO Specify limit?
        /// <summary>
        /// Get vlogs from the specified user.
        /// </summary>
        [HttpGet("users/{userId}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<VlogOutputModel>))]
        public async Task<IActionResult> ListForUserAsync([FromRoute]Guid userId)
        {
            var vlogs = await _vlogRepository.GetVlogsByUserAsync(userId);

            // Map the vlogs to their output model representations.
            IEnumerable<VlogOutputModel> output = vlogs
                .Select(v =>
                {
                    VlogOutputModel outputModel = v;
                    return outputModel;
                });

            return Ok(output);
        }

        /// <summary>
        /// Delete a vlog that is owned by the authenticated user.
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{vlogId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> DeleteAsync([FromRoute]Guid vlogId)
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
        public async Task<IActionResult> LikeAsync([FromRoute]Guid vlogId)
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
        public async Task<IActionResult> UnlikeAsync([FromRoute]Guid vlogId)
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