using Swabbr.Core.Extensions;
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
using Swabbr.Core.Types;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Swabbr.Api.Controllers
{
    /// <summary>
    ///     Controller for handling requests vlog operations.
    /// </summary>
    [Authorize]
    [ApiController]
    [ApiVersion("1")]
    [Route("api/{version:apiVersion}/vlogs")]
    public sealed class VlogsController : ControllerBase
    {
        private readonly IVlogService _vlogService;
        private readonly IUserService _userService;
        private readonly UserManager<SwabbrIdentityUser> _userManager;
        private readonly ILogger logger;

        /// <summary>
        ///     Create new instance.
        /// </summary>
        public VlogsController(IVlogService vlogService,
            IUserService userService,
            UserManager<SwabbrIdentityUser> userManager,
            ILoggerFactory loggerFactory)
        {
            _vlogService = vlogService ?? throw new ArgumentNullException(nameof(vlogService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            logger = (loggerFactory != null) ? loggerFactory.CreateLogger(nameof(VlogsController)) : throw new ArgumentNullException(nameof(loggerFactory));
        }

        /// <summary>
        ///     Gets a single <see cref="Vlog"/> from our data store, including it's
        ///     likes and thumbnail download uri.
        /// </summary>
        /// <remarks>
        ///     The thumbnail download uri is an Azure Storage SAS Uri.
        /// </remarks>
        /// <param name="vlogId">Internal <see cref="Vlog"/> id</param>
        /// <returns><see cref="VlogWrapperOutputModel"/></returns>
        [HttpGet("{vlogId}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(VlogWrapperOutputModel))]
        public async Task<IActionResult> GetAsync([FromRoute]Guid vlogId)
        {
            try
            {
                if (vlogId.IsEmpty()) { return BadRequest(this.Error(ErrorCodes.InvalidInput, "Vlog id can't be null or empty")); }

                var vlogWithThumbnailDetails = await _vlogService.GetWithThumbnailAsync(vlogId);
                var vlogLikeSummary = await _vlogService.GetVlogLikeSummaryForVlogAsync(vlogId);

                return Ok(new VlogWrapperOutputModel
                {
                    ThumbnailUri = vlogWithThumbnailDetails.ThumbnailUri,
                    Vlog = MapperVlog.Map(vlogWithThumbnailDetails.Vlog),
                    VlogLikeSummary = new VlogLikeSummaryOutputModel
                    {
                        VlogId = vlogLikeSummary.VlogId,
                        TotalLikes = vlogLikeSummary.TotalLikes,
                        SimplifiedUsers = vlogLikeSummary.Users.Select(x => new UserSimplifiedOutputModel
                        {
                            Id = x.Id,
                            NickName = x.Nickname
                        })
                    }
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
                if (input is null)
                {
                    throw new ArgumentNullException(nameof(input));
                }

                var user = await _userManager.GetUserAsync(User);
                var vlog = await _vlogService.GetAsync(vlogId);

                // Map properties
                vlog.IsPrivate = input.IsPrivate;

                // Act.
                await _vlogService.UpdateAsync(vlog);
                var updatedVlog = await _vlogService.GetAsync(vlogId);

                // Map.
                var result = MapperVlog.Map(updatedVlog);

                // Return.
                return Ok(result);
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

        // TODO This is too much for a controller to do.
        /// <summary>
        ///     Gets all vlogs for a given <see cref="SwabbrUser"/> including their 
        ///     thumbnail details and like details.
        /// </summary>
        /// <param name="userId">Internal <see cref="SwabbrUser"/> id</param>
        /// <returns><see cref="OkObjectResult"/> with <see cref="VlogCollectionOutputModel"/></returns>
        [HttpGet("foruser/{userId}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(VlogCollectionOutputModel))]
        public async Task<IActionResult> ListForUserAsync([FromRoute]Guid userId)
        {
            try
            {
                if (userId.IsEmpty()) { return BadRequest(this.Error(ErrorCodes.InvalidInput, "User id can't be null or empty")); }

                // Get all vlogs.
                var vlogsWithThumbnails = await _vlogService.GetVlogsByUserWithThumbnailsAsync(userId, Navigation.Default).ToListAsync();

                var mappedVlogs = new ConcurrentBag<VlogWrapperOutputModel>();
                Parallel.ForEach(vlogsWithThumbnails, (vlogWithThumbnail) =>
                {
                    var vlogLikeSummary = Task.Run(() => _vlogService.GetVlogLikeSummaryForVlogAsync(vlogWithThumbnail.Vlog.Id)).Result;
                    mappedVlogs.Add(new VlogWrapperOutputModel
                    {
                        Vlog = MapperVlog.Map(vlogWithThumbnail.Vlog),
                        ThumbnailUri = vlogWithThumbnail.ThumbnailUri,
                        VlogLikeSummary = new VlogLikeSummaryOutputModel
                        {
                            VlogId = vlogLikeSummary.VlogId,
                            TotalLikes = vlogLikeSummary.TotalLikes,
                            SimplifiedUsers = vlogLikeSummary.Users.Select(x => new UserSimplifiedOutputModel
                            {
                                Id = x.Id,
                                NickName = x.Nickname
                            })
                        }
                    });
                });

                // Wrap and return.
                return Ok(new VlogCollectionOutputModel
                {
                    Vlogs = mappedVlogs
                });
            }
            catch (EntityNotFoundException e)
            {
                logger.LogError(e.Message);
                return Conflict(this.Error(ErrorCodes.EntityNotFound, "Could not find object"));
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
                if (vlogId.IsEmpty()) { return BadRequest(this.Error(ErrorCodes.InvalidInput, "Vlog id can't be null or empty")); }

                var user = await _userManager.GetUserAsync(User);

                await _vlogService.DeleteAsync(vlogId);

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
                if (vlogId.IsEmpty()) { return BadRequest(this.Error(ErrorCodes.InvalidInput, "Vlog id can't be null or empty")); }
                var user = await _userManager.GetUserAsync(User);

                await _vlogService.LikeAsync(vlogId);
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
                if (vlogId.IsEmpty()) { return BadRequest(this.Error(ErrorCodes.InvalidInput, "Vlog id can't be null or empty")); }
                var user = await _userManager.GetUserAsync(User);

                await _vlogService.UnlikeAsync(vlogId);
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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(VlogLikesWithUsersOutputModel))]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        public async Task<IActionResult> GetLikesForVlogAsync([FromRoute] Guid vlogId)
        {
            try
            {
                if (vlogId.IsEmpty()) { return BadRequest(this.Error(ErrorCodes.InvalidInput, "Vlog id can't be null or empty")); }
                if (!await _vlogService.ExistsAsync(vlogId)) { return BadRequest(this.Error(ErrorCodes.EntityNotFound, "Vlog doesn't exist")); }

                var vlogLikes = await _vlogService.GetVlogLikesForVlogAsync(vlogId, Navigation.All).ToListAsync();

                // TODO Clean up
                var users = new List<SwabbrUserWithStats>();
                foreach (var userId in vlogLikes.Select(x => x.Id.UserId))
                {
                    users.Add(await _userService.GetWithStatisticsAsync(userId));
                }

                return Ok(new VlogLikesWithUsersOutputModel
                {
                    TotalLikeCount = vlogLikes.Count(),
                    UsersSimplified = users.Select(x => new UserSimplifiedOutputModel
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
        /// Gets a collection of recommended <see cref="Vlog"/>s for the logged in 
        /// <see cref="SwabbrUser"/>. This currently gets a list of most recent 
        /// <see cref="Vlog"/>s posted by <see cref="SwabbrUser"/>s that are being
        /// followed by the currently logged in <see cref="SwabbrUser"/>.
        /// </summary>
        /// <param name="maxCount">Maximum result set count</param>
        /// <returns><see cref="VlogCollectionOutputModel"/></returns>
        [HttpGet("recommended")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(VlogCollectionOutputModel))]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        public async Task<IActionResult> GetRecommendedVlogsAsync(uint maxCount = 25)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                // Get all vlogs.
                var navigation = new Navigation
                {
                    Limit = maxCount,
                    Offset = 0,
                };
                var vlogsWithThumbnails = await _vlogService.GetRecommendedForUserWithThumbnailsAsync(navigation).ToListAsync();
                var mappedVlogs = new ConcurrentBag<VlogWrapperOutputModel>();

                // TODO Use await foreach
                // Process each vlog separately.
                // TODO Duplicate code.
                Parallel.ForEach(vlogsWithThumbnails, (vlogWithThumbnail) =>
                {
                    var vlogLikeSummary = Task.Run(() => _vlogService.GetVlogLikeSummaryForVlogAsync(vlogWithThumbnail.Vlog.Id)).Result;
                    mappedVlogs.Add(new VlogWrapperOutputModel
                    {
                        Vlog = MapperVlog.Map(vlogWithThumbnail.Vlog),
                        ThumbnailUri = vlogWithThumbnail.ThumbnailUri,
                        VlogLikeSummary = new VlogLikeSummaryOutputModel
                        {
                            VlogId = vlogLikeSummary.VlogId,
                            TotalLikes = vlogLikeSummary.TotalLikes,
                            SimplifiedUsers = vlogLikeSummary.Users.Select(x => new UserSimplifiedOutputModel
                            {
                                Id = x.Id,
                                NickName = x.Nickname
                            })
                        }
                    });
                });

                // Wrap and return.
                return Ok(new VlogCollectionOutputModel
                {
                    Vlogs = mappedVlogs
                });
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return Conflict(this.Error(ErrorCodes.InvalidOperation, "Could not get recommended vlogs for user"));
            }
        }

        /// <summary>
        /// Gets downstream parameters, including token, for playback of a specified
        /// <see cref="Core.Entities.Vlog"/>.
        /// </summary>
        /// <param name="vlogId">Internal <see cref="Core.Entities.Vlog"/> id</param>
        /// <returns><see cref="VlogPlaybackDetailsOutputModel"/></returns>
        [HttpGet("{vlogId}/watch")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(VlogPlaybackDetailsOutputModel))]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        public async Task<IActionResult> WatchAsync([FromRoute]Guid vlogId)
        {
            try
            {
                if (vlogId.IsEmpty()) { Conflict(this.Error(ErrorCodes.InvalidInput, "Vlog id is invalid or missing")); }

                var user = await _userManager.GetUserAsync(User);

                // TODO Build
                throw new NotImplementedException();
            }
            catch (EntityNotFoundException e)
            {
                logger.LogError(e.Message);
                return Conflict(this.Error(ErrorCodes.EntityNotFound, "Could not find vlog"));
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return Conflict(this.Error(ErrorCodes.InvalidOperation, "Could not get playback details for vlog"));
            }
        }
    }
}
