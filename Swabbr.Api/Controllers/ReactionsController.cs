using Swabbr.Core.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swabbr.Api.Authentication;
using Swabbr.Api.Errors;
using Swabbr.Api.Extensions;
using Swabbr.Api.Mapping;
using Swabbr.Api.ViewModels;
using Swabbr.Api.ViewModels.Reaction;
using Swabbr.Core.Entities;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Swabbr.Core.Types;

namespace Swabbr.Api.Controllers
{
    /// <summary>
    ///     Controller for handling requests related to reactions.
    /// </summary>
    [Authorize]
    [ApiController]
    [ApiVersion("1")]
    [Route("api/{version:apiVersion}/reactions")]
    public class ReactionsController : ControllerBase
    {
        private readonly UserManager<SwabbrIdentityUser> _userManager;
        private readonly IReactionService _reactionService;
        private readonly IVlogService _vlogService;
        private readonly ILogger logger;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public ReactionsController(UserManager<SwabbrIdentityUser> userManager,
            IReactionService reactionService,
            IVlogService vlogService,
            ILoggerFactory loggerFactory)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _reactionService = reactionService ?? throw new ArgumentNullException(nameof(reactionService));
            _vlogService = vlogService ?? throw new ArgumentNullException(nameof(vlogService));
            logger = (loggerFactory != null) ? loggerFactory.CreateLogger(nameof(ReactionsController)) : throw new ArgumentNullException(nameof(loggerFactory));
        }

        /// <summary>
        /// Deletes a <see cref="Reaction"/> from our backend.
        /// </summary>
        /// <param name="reactionId">Internal <see cref="Reaction"/> id</param>
        /// <returns><see cref="NoContentResult"/></returns>
        [HttpDelete("{reactionId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> Delete([FromRoute] Guid reactionId)
        {
            try
            {
                if (reactionId.IsNullOrEmpty()) { return BadRequest(this.Error(ErrorCodes.InvalidInput, "Reaction id can't be null or empty")); }

                var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);

                // TODO How to ensure we are allowed to this? --> do we own the vlog?
                await _reactionService.DeleteReactionAsync(reactionId).ConfigureAwait(false);
                return NoContent();
            }
            catch (NotAllowedException e)
            {
                logger.LogError(e.Message);
                return Conflict(this.Error(ErrorCodes.InsufficientAccessRights, "User does not own reaction"));
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return Conflict(this.Error(ErrorCodes.InvalidOperation, "Could not delete reaction"));
            }
        }

        /// <summary>
        /// Gets a single <see cref="Reaction"/>.
        /// </summary>
        /// <param name="reactionId">Internal <see cref="Reaction"/> id</param>
        /// <returns><see cref="OkObjectResult"/> with <see cref="ReactionOutputModel"/></returns>
        [HttpGet("{reactionId}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ReactionWrapperOutputModel))]
        public async Task<IActionResult> Get([FromRoute] Guid reactionId)
        {
            try
            {
                if (reactionId.IsNullOrEmpty()) { return BadRequest(this.Error(ErrorCodes.InvalidInput, "Reaction id can't be null or empty")); }

                var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);

                var reactionWithThumbnail = await _reactionService.GetWithThumbnailAsync(reactionId);
                return Ok(new ReactionWrapperOutputModel
                {
                    Reaction = MapperReaction.Map(reactionWithThumbnail.Reaction),
                    ThumbnailUri = reactionWithThumbnail.ThumbnailUri
                });
            }
            catch (EntityNotFoundException e)
            {
                logger.LogError(e.Message);
                return Conflict(this.Error(ErrorCodes.EntityNotFound, "Could not find reaction"));
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return Conflict(this.Error(ErrorCodes.InvalidOperation, "Could not get reaction"));
            }
        }

        /// <summary>
        /// Lists all <see cref="Reaction"/>s for a given <see cref="Vlog"/>.
        /// </summary>
        /// <param name="vlogId">Internal <see cref="Vlog"/> id</param>
        /// <returns><see cref="OkObjectResult"/> with <see cref="ReactionCollectionOutputModel"/></returns>
        [HttpGet("for_vlog/{vlogId}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<ReactionCollectionOutputModel>))]
        public async Task<IActionResult> GetReactionsForVlog([FromRoute] Guid vlogId)
        {
            try
            {
                if (vlogId.IsNullOrEmpty()) { return BadRequest(this.Error(ErrorCodes.InvalidInput, "Vlog id can't be null or empty")); }
                if (!await _vlogService.ExistsAsync(vlogId).ConfigureAwait(false)) { return BadRequest(this.Error(ErrorCodes.EntityNotFound, "Vlog doesn't exist")); }

                var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);

                var reactions = await _reactionService.GetReactionsForVlogWithThumbnailsAsync(vlogId, Navigation.Default).ToListAsync();
                return Ok(new ReactionCollectionOutputModel
                {
                    Reactions = reactions.Select(x => new ReactionWrapperOutputModel
                    {
                        Reaction = MapperReaction.Map(x.Reaction),
                        ThumbnailUri = x.ThumbnailUri
                    }),
                    ReactionTotalCount = (uint)reactions.Count()
                });
            }
            catch (EntityNotFoundException e)
            {
                logger.LogError(e.Message);
                return Conflict(this.Error(ErrorCodes.EntityNotFound, "Could not find vlog"));
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return Conflict(this.Error(ErrorCodes.InvalidOperation, "Could not get reactions for vlog"));
            }
        }

        /// <summary>
        /// Gets only the <see cref="Reaction"/> count for a <see cref="Vlog"/>.
        /// </summary>
        /// <param name="vlogId">Internal <see cref="Vlog"/> id</param>
        /// <returns><see cref="ReactionOutputModel"/></returns>
        [HttpGet("for_vlog/{vlogId}/count")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<ReactionOutputModel>))]
        public async Task<IActionResult> GetReactionCountForVlog([FromRoute] Guid vlogId)
        {
            try
            {
                if (vlogId.IsNullOrEmpty()) { return BadRequest(this.Error(ErrorCodes.InvalidInput, "Vlog id can't be null or empty")); }
                if (!await _vlogService.ExistsAsync(vlogId).ConfigureAwait(false)) { return BadRequest(this.Error(ErrorCodes.EntityNotFound, "Vlog doesn't exist")); }

                var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);

                return Ok(new ReactionCountOutputModel
                {
                    ReactionCount = await _reactionService.GetReactionCountForVlogAsync(vlogId).ConfigureAwait(false)
                });
            }
            catch (EntityNotFoundException e)
            {
                logger.LogError(e.Message);
                return Conflict(this.Error(ErrorCodes.EntityNotFound, "Could not find vlog"));
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return Conflict(this.Error(ErrorCodes.InvalidOperation, "Could not get reactions for vlog"));
            }
        }

        /// <summary>
        ///     Create a new reaction to a vlog.
        /// </summary>
        /// <remarks>
        ///     This should be called after the reaction file has been uploaded.
        /// </remarks>
        /// <param name="model">Input model for posting a reaction.</param>
        /// <returns>The actual created reaction object.</returns>
        [HttpPost("new")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ReactionOutputModel))]
        public async Task<IActionResult> PostReactionAsync([FromBody] ReactionInputModel model)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);

                // Act.
                await _reactionService.PostReactionAsync(model.TargetVlogId, model.ReactionId).ConfigureAwait(false);
                var postedReaction = await _reactionService.GetAsync(model.ReactionId);

                // Map.
                var result = MapperReaction.Map(postedReaction);

                // Return.
                return Ok(result);
            }
            catch (EntityNotFoundException e)
            {
                logger.LogError(e.Message);
                return Conflict(this.Error(ErrorCodes.EntityNotFound, "Could not find target vlog"));
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return Conflict(this.Error(ErrorCodes.InvalidOperation, "Could not post new reaction"));
            }
        }

        /// <summary>
        /// Updates a <see cref="Reaction"/> in our data store.
        /// </summary>
        /// <param name="reactionId">Internal <see cref="Reaction"/> id</param>
        /// <param name="model"><see cref="ReactionUpdateInputModel"/></param>
        /// <returns><see cref="OkObjectResult"/> with <see cref="ReactionOutputModel"/></returns>
        [HttpPost("{reactionId}/update")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ReactionOutputModel))]
        public async Task<IActionResult> Update([FromRoute] Guid reactionId, [FromBody] ReactionUpdateInputModel model)
        {
            try
            {
                if (model is null)
                {
                    throw new ArgumentNullException(nameof(model));
                }

                // Act.
                // TODO Reaction update operation
                var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);

                var reaction = await _reactionService.GetAsync(reactionId).ConfigureAwait(false);
                reaction.IsPrivate = model.IsPrivate;

                await _reactionService.UpdateReactionAsync(reaction).ConfigureAwait(false);
                var updatedReaction = await _reactionService.GetAsync(reactionId).ConfigureAwait(false);

                // Map.
                var result = MapperReaction.Map(updatedReaction);

                // Return.
                return Ok(result);
            }
            catch (NotAllowedException e)
            {
                logger.LogError(e.Message);
                return Conflict(this.Error(ErrorCodes.InsufficientAccessRights, "User does not own reaction"));
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return Conflict(this.Error(ErrorCodes.InvalidOperation, "Could not update reaction"));
            }
        }

        /// <summary>
        /// Gets downstream parameters, including token, for playback of a specified
        /// <see cref="Core.Entities.Reaction"/>.
        /// </summary>
        /// <param name="reactionId">Internal <see cref="Core.Entities.Reaction"/> id</param>
        /// <returns><see cref="ReactionPlaybackDetailsOutputModel"/></returns>
        [HttpGet("{reactionId}/watch")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ReactionPlaybackDetailsOutputModel))]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        public async Task<IActionResult> GetPlayackDetailsAsync([FromRoute]Guid reactionId)
        {
            try
            {
                if (reactionId.IsNullOrEmpty()) { Conflict(this.Error(ErrorCodes.InvalidInput, "Vlog id is invalid or missing")); }

                var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);

                // TODO Implement
                throw new NotImplementedException();
            }
            catch (ReactionStateException e)
            {
                logger.LogError(e.Message);
                return Conflict(this.Error(ErrorCodes.InvalidOperation, "Reaction is not yet ready for playback"));
            }
            catch (EntityNotFoundException e)
            {
                logger.LogError(e.Message);
                return Conflict(this.Error(ErrorCodes.EntityNotFound, "Could not find reaction"));
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return Conflict(this.Error(ErrorCodes.InvalidOperation, "Could not get playback details for reaction"));
            }
        }
    }
}
