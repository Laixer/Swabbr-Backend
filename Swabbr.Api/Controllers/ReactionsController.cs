using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swabbr.Api.Authentication;
using Swabbr.Api.Errors;
using Swabbr.Api.Extensions;
using Swabbr.Api.ViewModels;
using Swabbr.Core.Entities;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Interfaces;
using Swabbr.Core.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Swabbr.Api.Controllers
{
    /// <summary>
    /// Controller for handling requests related to vlog reactions.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("reactions")]
    public class ReactionsController : ControllerBase
    {
        private readonly UserManager<SwabbrIdentityUser> _userManager;
        private readonly IReactionRepository _reactionRepository;
        private readonly IVlogRepository _vlogRepository;

        public ReactionsController(
            UserManager<SwabbrIdentityUser> userManager,
            IReactionRepository reactionRepository,
            IVlogRepository vlogRepository
            )
        {
            _userManager = userManager;
            _reactionRepository = reactionRepository;
            _vlogRepository = vlogRepository;
        }

        /// <summary>
        /// Create a new reaction to a vlog.
        /// </summary>
        [HttpPost("create")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ReactionOutputModel))]
        public async Task<IActionResult> Create([FromBody] ReactionInputModel inputModel)
        {
            throw new NotImplementedException();
            //var identityUser = await _userManager.GetUserAsync(User);

            ////TODO: Check if user has access to post a reaction to the given vlog

            //var newReaction = new Reaction
            //{
            //    VlogId = inputModel.VlogId,
            //    UserId = identityUser.Id,
            //    IsPrivate = inputModel.IsPrivate,
            //    DatePosted = DateTime.Now,
            //};

            //// Create and return the reaction entity
            //ReactionOutputModel output = await _reactionRepository.CreateAsync(newReaction);
            //return Ok(output);
        }

        /// <summary>
        /// Update an existing reaction to a vlog.
        /// </summary>
        [HttpPut("update")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ReactionOutputModel))]
        [ProducesResponseType((int)HttpStatusCode.Forbidden, Type = typeof(ErrorMessage))]
        public async Task<IActionResult> Update([FromBody] ReactionUpdateInputModel inputModel)
        {
            throw new NotImplementedException();
            //var identityUser = await _userManager.GetUserAsync(User);

            //var reaction = await _reactionRepository.GetByIdAsync(inputModel.ReactionId);

            //if (!reaction.UserId.Equals(identityUser.Id))
            //{
            //    return StatusCode(
            //        (int)HttpStatusCode.Forbidden,
            //        this.Error(ErrorCodes.InsufficientAccessRights, "User is not allowed to perform this action.")
            //    );
            //}

            //// Update the reaction entity
            //reaction.IsPrivate = inputModel.IsPrivate;

            //ReactionOutputModel output = await _reactionRepository.UpdateAsync(reaction);
            //return Ok(output);
        }

        /// <summary>
        /// Get reactions to a vlog.
        /// </summary>
        [HttpGet("vlogs/{vlogId}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<ReactionOutputModel>))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ErrorMessage))]
        public async Task<IActionResult> GetReactionsForVlog([FromRoute] Guid vlogId)
        {
            throw new NotImplementedException();
            //if (!(await _vlogRepository.ExistsAsync(vlogId)))
            //{
            //    return NotFound(
            //        this.Error(ErrorCodes.EntityNotFound, "Vlog does not exist.")
            //        );
            //}

            //var reactions = await _reactionRepository.GetReactionsForVlogAsync(vlogId);

            //// Map the collection to output models.
            //IEnumerable<ReactionOutputModel> output = reactions
            //    .Select(entity => (ReactionOutputModel)entity);

            //return Ok(output);
        }

        /// <summary>
        /// Get a single reaction to a vlog by id.
        /// </summary>
        [HttpGet("{reactionId}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ReactionOutputModel))]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ErrorMessage))]
        public async Task<IActionResult> Get([FromRoute] Guid reactionId)
        {
            throw new NotImplementedException();
            //try
            //{
            //    ReactionOutputModel output = await _reactionRepository.GetByIdAsync(reactionId);
            //    return Ok(output);
            //}
            //catch (EntityNotFoundException)
            //{
            //    return NotFound(
            //        this.Error(ErrorCodes.EntityNotFound, "Reaction could not be found.")
            //        );
            //}
        }

        /// <summary>
        /// Delete a reaction to a vlog for the authenticated user.
        /// </summary>
        [HttpDelete("{reactionId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound, Type = typeof(ErrorMessage))]
        [ProducesResponseType((int)HttpStatusCode.Forbidden, Type = typeof(ErrorMessage))]
        public async Task<IActionResult> Delete([FromRoute] Guid reactionId)
        {
            throw new NotImplementedException();
            //try
            //{
            //    var identityUser = await _userManager.GetUserAsync(User);

            //    var reaction = await _reactionRepository.GetByIdAsync(reactionId);

            //    if (!identityUser.Id.Equals(reaction.UserId))
            //    {
            //        return StatusCode(
            //            (int)HttpStatusCode.Forbidden,
            //            this.Error(ErrorCodes.InsufficientAccessRights, "User is not allowed to perform this action.")
            //        );
            //    }

            //    await _reactionRepository.DeleteAsync(reaction);
            //    return NoContent();
            //}
            //catch (EntityNotFoundException)
            //{
            //    return NotFound(
            //        this.Error(ErrorCodes.EntityNotFound, "Reaction could not be found.")
            //        );
            //}
        }
    }
}