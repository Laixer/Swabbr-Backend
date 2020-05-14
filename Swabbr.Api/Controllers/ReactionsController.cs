﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swabbr.Api.Authentication;
using Swabbr.Api.Errors;
using Swabbr.Api.Extensions;
using Swabbr.Api.Mapping;
using Swabbr.Api.ViewModels;
using Swabbr.Core.Entities;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Laixer.Utility.Extensions;
using Swabbr.Api.ViewModels.Reaction;
using System.Linq;

namespace Swabbr.Api.Controllers
{

    /// <summary>
    /// Controller for handling requests related to <see cref="Vlog"/> <see cref="Reaction"/>s.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("reactions")]
    public class ReactionsController : ControllerBase
    {

        private readonly UserManager<SwabbrIdentityUser> _userManager;
        private readonly IPlaybackService _livestreamPlaybackService;
        private readonly IReactionService _reactionService;
        private readonly IVlogService _vlogService;
        private readonly ILogger logger;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public ReactionsController(UserManager<SwabbrIdentityUser> userManager,
            IPlaybackService livestreamPlaybackService,
            IReactionService reactionService,
            IVlogService vlogService,
            ILoggerFactory loggerFactory)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _livestreamPlaybackService = livestreamPlaybackService ?? throw new ArgumentNullException(nameof(livestreamPlaybackService));
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

                await _reactionService.DeleteReactionAsync(user.Id, reactionId).ConfigureAwait(false);
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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ReactionOutputModel))]
        public async Task<IActionResult> Get([FromRoute] Guid reactionId)
        {
            try
            {
                if (reactionId.IsNullOrEmpty()) { return BadRequest(this.Error(ErrorCodes.InvalidInput, "Reaction id can't be null or empty")); }

                var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);

                return Ok(MapperReaction.Map(await _reactionService.GetReactionAsync(reactionId).ConfigureAwait(false)));
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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<ReactionOutputModel>))]
        public async Task<IActionResult> GetReactionsForVlog([FromRoute] Guid vlogId)
        {
            try
            {
                if (vlogId.IsNullOrEmpty()) { return BadRequest(this.Error(ErrorCodes.InvalidInput, "Vlog id can't be null or empty")); }
                if (!await _vlogService.ExistsAsync(vlogId).ConfigureAwait(false)) { return BadRequest(this.Error(ErrorCodes.EntityNotFound, "Vlog doesn't exist")); }

                var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);

                return Ok(new ReactionCollectionOutputModel
                {
                    Reactions = (await _reactionService
                        .GetReactionsForVlogAsync(vlogId)
                        .ConfigureAwait(false))
                        .Select(x => MapperReaction.Map(x))
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
        /// Create a new reaction to a vlog.
        /// </summary>
        /// <param name="model"><see cref="ReactionInputModel"/></param>
        /// <returns><see cref="OkObjectResult"/> with <see cref="ReactionOutputModel"/></returns>
        [HttpPost("new")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ReactionOutputModel))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        public async Task<IActionResult> Post([FromBody] ReactionInputModel model)
        {
            try
            {
                if (model == null) { return BadRequest(this.Error(ErrorCodes.InvalidInput, "Model can't be null")); }
                if (!ModelState.IsValid) { return BadRequest(this.Error(ErrorCodes.InvalidInput, "Model state is not valid")); }

                var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);

                return Ok(await _reactionService.PostReactionAsync(user.Id, model.TargetVlogId, model.IsPrivate).ConfigureAwait(false));
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
        /// Refreshes the <see cref="Reaction"/> upload uri.
        /// </summary>
        /// <param name="reactionId">Internal <see cref="Reaction"/> id</param>
        /// <returns><see cref="OkObjectResult"/></returns>
        [HttpGet("refresh_upload_url")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ReactionUploadUrlOutputModel))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        public async Task<IActionResult> RefreshUploadUrlAsync(Guid reactionId)
        {
            if (reactionId.IsNullOrEmpty()) { return BadRequest(this.Error(ErrorCodes.InvalidInput, "Reaction id can't be null")); }

            try
            {
                var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
                return Ok(new ReactionUploadUrlOutputModel
                {
                    UploadUri = await _reactionService.GetNewUploadUriAsync(user.Id, reactionId).ConfigureAwait(false)
                });
            }
            catch (EntityNotFoundException e)
            {
                logger.LogError(e.Message);
                return Conflict(this.Error(ErrorCodes.EntityNotFound, "Could not find reaction"));
            }
            catch (ReactionStateException e)
            {
                logger.LogError(e.Message);
                return Conflict(this.Error(ErrorCodes.InvalidOperation, "Reaction not in upload awaiting state"));
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return Conflict(this.Error(ErrorCodes.InvalidOperation, "Could not post new reaction"));
            }
        }

        /// <summary>
        /// Should be called when the user finishes uploading a <see cref="Reaction"/>.
        /// </summary>
        /// <param name="model"><see cref="ReactionInputModel"/></param>
        /// <returns><see cref="OkObjectResult"/> with <see cref="ReactionOutputModel"/></returns>
        [HttpPost("finished_uploading")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        public async Task<IActionResult> OnFinishedUploading(Guid reactionId)
        {
            if (reactionId.IsNullOrEmpty()) { return BadRequest(this.Error(ErrorCodes.InvalidInput, "Reaction id can't be null")); }

            try
            {
                var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
                await _reactionService.OnFinishedUploadingReactionAsync(reactionId).ConfigureAwait(false);
                return Ok();
            }
            catch (EntityNotFoundException e)
            {
                logger.LogError(e.Message);
                return Conflict(this.Error(ErrorCodes.EntityNotFound, "Could not find reaction"));
            }
            catch (ReactionStateException e)
            {
                logger.LogError(e.Message);
                return Conflict(this.Error(ErrorCodes.InvalidOperation, "Reaction not in upload awaiting state"));
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
                if (reactionId.IsNullOrEmpty()) { return BadRequest(this.Error(ErrorCodes.InvalidInput, "Reaction id can't be null or empty")); }
                if (model == null) { return BadRequest(this.Error(ErrorCodes.InvalidInput, "Model can't be null")); }
                if (!ModelState.IsValid) { return BadRequest(this.Error(ErrorCodes.InvalidInput, "Model state is not valid")); }

                var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);

                var reaction = await _reactionService.UpdateReactionAsync(user.Id, reactionId, model.IsPrivate).ConfigureAwait(false);
                return Ok(MapperReaction.Map(reaction));
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

                var pars = await _livestreamPlaybackService.GetReactionDownstreamParametersAsync(reactionId, user.Id).ConfigureAwait(false);
                return Ok(new ReactionPlaybackDetailsOutputModel
                {
                    EndpointUrl = pars.EndpointUrl,
                    Token = pars.Token,
                    ReactionId = pars.ReactionId
                });
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