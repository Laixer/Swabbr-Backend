using Laixer.Utility.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swabbr.Api.Authentication;
using Swabbr.Api.Errors;
using Swabbr.Api.Extensions;
using Swabbr.Api.Mapping;
using Swabbr.Api.ViewModels.User;
using Swabbr.Api.ViewModels.Vlog;
using Swabbr.Api.ViewModels.VlogLike;
using Swabbr.Core.Entities;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Interfaces.Services;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Swabbr.Api.Controllers
{

    /// <summary>
    /// Controller for handling requests related to <see cref="Vlog"/> entities.
    /// TODO Private functionality for vlogs
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("vlogs")]
    public sealed class VlogsController : ControllerBase
    {

        private readonly IVlogService _vlogService;
        private readonly IUserWithStatsService _userWithStatsService;
        private readonly UserManager<SwabbrIdentityUser> _userManager;
        private readonly ILogger logger;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public VlogsController(IVlogService vlogService,
            IUserWithStatsService userWithStatsService,
            UserManager<SwabbrIdentityUser> userManager,
            ILoggerFactory loggerFactory)
        {
            _vlogService = vlogService ?? throw new ArgumentNullException(nameof(vlogService));
            _userWithStatsService = userWithStatsService ?? throw new ArgumentNullException(nameof(userWithStatsService));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            logger = (loggerFactory != null) ? loggerFactory.CreateLogger(nameof(VlogsController)) : throw new ArgumentNullException(nameof(loggerFactory));
        }

        /// <summary>
        /// Gets a single <see cref="Vlog"/> from our data store.
        /// </summary>
        /// <param name="vlogId">Internal <see cref="Vlog"/> id</param>
        /// <returns></returns>
        [HttpGet("{vlogId}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(VlogOutputModel))]
        public async Task<IActionResult> GetAsync([FromRoute]Guid vlogId)
        {
            try
            {
                if (vlogId.IsNullOrEmpty()) { return BadRequest(this.Error(ErrorCodes.InvalidInput, "Vlog id can't be null or empty")); }

                return Ok(new VlogWithLikesOutputModel
                {
                    Vlog = MapperVlog.Map(
                        await _vlogService
                        .GetAsync(vlogId)
                        .ConfigureAwait(false)),
                    VlogLikes = (await _vlogService
                        .GetVlogLikesForVlogAsync(vlogId)
                        .ConfigureAwait(false))
                        .Select(x => MapperVlogLike.Map(x))
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
                return Conflict(this.Error(ErrorCodes.InvalidOperation, "Could not get vlog"));
            }
        }

        /// <summary>
        /// Updates the details for a <see cref="Vlog"/>.
        /// </summary>
        /// <param name="vlogId">Internal <see cref="Vlog"/> id</param>
        /// <param name="input"><see cref="VlogUpdateInputModel"/></param>
        /// <returns><see cref="OkObjectResult"/> with updated <see cref="VlogOutputModel"/></returns>
        [HttpPost("{vlogId}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(VlogOutputModel))]
        public async Task<IActionResult> UpdateAsync([FromRoute]Guid vlogId, [FromBody]VlogUpdateInputModel input)
        {
            try
            {
                if (vlogId.IsNullOrEmpty()) { return BadRequest(this.Error(ErrorCodes.InvalidInput, "Vlog id can't be null or empty")); }
                if (input == null) { return BadRequest(this.Error(ErrorCodes.InvalidInput, "Body can't be null")); }
                if (!ModelState.IsValid) { return BadRequest(this.Error(ErrorCodes.InvalidInput, "Input model is invalid")); }

                var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);

                return Ok(MapperVlog.Map(await _vlogService.UpdateAsync(vlogId, user.Id, input.IsPrivate).ConfigureAwait(false)));
            }
            catch (EntityNotFoundException e)
            {
                logger.LogError(e.Message);
                return Conflict(this.Error(ErrorCodes.EntityNotFound, "Could not find vlog"));
            }
            catch (NotAllowedException e)
            {
                logger.LogError(e.Message);
                return Conflict(this.Error(ErrorCodes.InsufficientAccessRights, "User does not own vlog"));
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return Conflict(this.Error(ErrorCodes.InvalidOperation, "Could not update vlog"));
            }
        }

        /// <summary>
        /// Gets all vlogs for a given <see cref="SwabbrUser"/>.
        /// TODO This ignores <see cref="Vlog.IsPrivate"/>.
        /// </summary>
        /// <param name="userId">Internal <see cref="SwabbrUser"/> id</param>
        /// <returns><see cref="OkObjectResult"/> with <see cref="VlogCollectionOutputModel"/></returns>
        [HttpGet("foruser/{userId}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(VlogCollectionOutputModel))]
        public async Task<IActionResult> ListForUserAsync([FromRoute]Guid userId)
        {
            try
            {
                if (userId.IsNullOrEmpty()) { return BadRequest(this.Error(ErrorCodes.InvalidInput, "User id can't be null or empty")); }

                return Ok(new VlogCollectionOutputModel
                {
                    Vlogs = (await _vlogService
                        .GetVlogsFromUserAsync(userId)
                        .ConfigureAwait(false))
                        .Select(x => MapperVlog.Map(x))
                });
            }
            catch (EntityNotFoundException e)
            {
                logger.LogError(e.Message);
                return Conflict(this.Error(ErrorCodes.EntityNotFound, "Could not find object")); // TODO Specify
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return Conflict(this.Error(ErrorCodes.InvalidOperation, "Could not get vlogs for user"));
            }
        }

        /// <summary>
        /// Deletes a <see cref="Vlog"/> for the logged in <see cref="SwabbrUser"/>.
        /// </summary>
        /// <param name="vlogId">Internal <see cref="Vlog"/> id</param>
        /// <returns><see cref="OkObjectResult"/></returns>
        [HttpDelete("{vlogId}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteAsync([FromRoute]Guid vlogId)
        {
            try
            {
                if (vlogId.IsNullOrEmpty()) { return BadRequest(this.Error(ErrorCodes.InvalidInput, "Vlog id can't be null or empty")); }

                var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);

                await _vlogService.DeleteAsync(vlogId, user.Id).ConfigureAwait(false);

                return Ok();
            }
            catch (EntityNotFoundException e)
            {
                logger.LogError(e.Message);
                return Conflict(this.Error(ErrorCodes.EntityNotFound, "Could not find vlog"));
            }
            catch (NotAllowedException e)
            {
                logger.LogError(e.Message);
                return Conflict(this.Error(ErrorCodes.InsufficientAccessRights, "User does not own vlog"));
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return Conflict(this.Error(ErrorCodes.InvalidOperation, "Could not delete vlog"));
            }
        }

        /// <summary>
        /// Creates a new vlog-like relationship between a vlog and a user.
        /// </summary>
        /// <param name="vlogId">Internal <see cref="Vlog"/> id</param>
        /// <returns><see cref="OkResult"/></returns>
        [HttpPost("{vlogId}/like")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> LikeAsync([FromRoute]Guid vlogId)
        {
            try
            {
                if (vlogId.IsNullOrEmpty()) { return BadRequest(this.Error(ErrorCodes.InvalidInput, "Vlog id can't be null or empty")); }
                var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);

                await _vlogService.LikeAsync(vlogId, user.Id).ConfigureAwait(false);
                return Ok();
            }
            catch (EntityNotFoundException e)
            {
                logger.LogError(e.Message);
                return Conflict(this.Error(ErrorCodes.EntityNotFound, "Could not find vlog"));
            }
            catch (NotAllowedException e)
            {
                logger.LogError(e.Message);
                return Conflict(this.Error(ErrorCodes.InsufficientAccessRights, "You are not allowed to like your own vlog"));
            }
            catch (OperationAlreadyExecutedException e)
            {
                logger.LogError(e.Message);
                return Conflict(this.Error(ErrorCodes.InsufficientAccessRights, "User has already liked this vlog"));
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return Conflict(this.Error(ErrorCodes.InvalidOperation, "Could not like vlog"));
            }
        }

        /// <summary>
        /// Removes a new vlog-like relationship between a vlog and a user.
        /// </summary>
        /// <param name="vlogId">Internal <see cref="Vlog"/> id</param>
        /// <returns><see cref="OkResult"/></returns>
        [HttpPost("{vlogId}/unlike")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> UnlikeAsync([FromRoute]Guid vlogId)
        {
            try
            {
                if (vlogId.IsNullOrEmpty()) { return BadRequest(this.Error(ErrorCodes.InvalidInput, "Vlog id can't be null or empty")); }
                var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);

                await _vlogService.UnlikeAsync(vlogId, user.Id).ConfigureAwait(false);
                return Ok();
            }
            catch (EntityNotFoundException e)
            {
                logger.LogError(e.Message);
                return Conflict(this.Error(ErrorCodes.EntityNotFound, "Could not find vlog-like relationship"));
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return Conflict(this.Error(ErrorCodes.InvalidOperation, "Could not unlike vlog"));
            }
        }

        /// <summary>
        /// Gets all <see cref="VlogLike"/>s including <see cref="SwabbrUser"/>s 
        /// for a given <paramref name="vlogId"/>.
        /// </summary>
        /// <param name="vlogId">Internal <see cref="Vlog"/> id</param>
        /// <returns><see cref="VlogLikesWithUsersOutputModel"/></returns>
        [HttpGet("{vlogId}/vlog_likes")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetLikesForVlogAsync([FromRoute] Guid vlogId)
        {
            try
            {
                if (vlogId.IsNullOrEmpty()) { return BadRequest(this.Error(ErrorCodes.InvalidInput, "Vlog id can't be null or empty")); }
                if (!await _vlogService.ExistsAsync(vlogId).ConfigureAwait(false)) { return BadRequest(this.Error(ErrorCodes.EntityNotFound, "Vlog doesn't exist")); }

                var vlogLikes = await _vlogService.GetVlogLikesForVlogAsync(vlogId).ConfigureAwait(false);
                var users = await _userWithStatsService.GetFromIdsAsync(vlogLikes.Select(x => x.UserId)).ConfigureAwait(false);

                return Ok(new VlogLikesWithUsersOutputModel
                {
                    TotalLikeCount = vlogLikes.Count(),
                    UsersMinified = users.Select(x => new UserMinifiedOutputModel
                    {
                        Id = x.Id,
                        NickName = x.Nickname
                    })
                });
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return Conflict(this.Error(ErrorCodes.InvalidOperation, "Could not get likes for vlog"));
            }
        }

        /// <summary>
        /// Gets a collection of recommended <see cref="Vlog"/>s for the logged in user.
        /// </summary>
        /// <returns><see cref="VlogCollectionOutputModel"/></returns>
        [HttpGet("recommended")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(VlogCollectionOutputModel))]
        public async Task<IActionResult> GetRecommendedVlogsAsync()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
                return Ok(new VlogCollectionOutputModel
                {
                    Vlogs = (await _vlogService.GetRecommendedForUserAsync(user.Id, 50)
                        .ConfigureAwait(false))
                        .Select(x => MapperVlog.Map(x))
                });
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return Conflict(this.Error(ErrorCodes.InvalidOperation, "Could not get recommended vlogs for user"));
            }
        }

    }

}
