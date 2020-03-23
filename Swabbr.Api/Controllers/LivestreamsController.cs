using Laixer.Utility.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swabbr.Api.Authentication;
using Swabbr.Api.Errors;
using Swabbr.Api.Extensions;
using Swabbr.Api.ViewModels;
using Swabbr.Api.ViewModels.Livestreaming;
using Swabbr.Api.ViewModels.Vlog;
using Swabbr.Core.Entities;
using Swabbr.Core.Enums;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Interfaces.Services;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Swabbr.Api.Controllers
{

    /// <summary>
    /// Contains all endpoints regarding <see cref="Livestream"/> entities.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("livestreams")]
    public class LivestreamsController : ControllerBase
    {

        private readonly UserManager<SwabbrIdentityUser> _userManager;
        private readonly ILivestreamService _livestreamingService;
        private readonly ILivestreamRepository _livestreamRepository; // TODO Is this really the controllers job? (used for checks only)
        private readonly IUserStreamingHandlingService _userStreamingHandlingService;
        private readonly ILogger logger;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public LivestreamsController(UserManager<SwabbrIdentityUser> userManager,
            ILivestreamService livestreamingService,
            ILivestreamRepository livestreamRepository,
            ILoggerFactory loggerFactory,
            IUserStreamingHandlingService userStreamingHandlingService)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _livestreamingService = livestreamingService ?? throw new ArgumentNullException(nameof(livestreamingService));
            _livestreamRepository = livestreamRepository ?? throw new ArgumentNullException(nameof(livestreamRepository));
            logger = (loggerFactory != null) ? loggerFactory.CreateLogger(nameof(LivestreamsController)) : throw new ArgumentNullException(nameof(loggerFactory));
            _userStreamingHandlingService = userStreamingHandlingService ?? throw new ArgumentNullException(nameof(userStreamingHandlingService));
        }

        /// <summary>
        /// Indicates that a user is going to start streaming to the given 
        /// <see cref="Livestream"/>.
        /// </summary>
        /// <param name="livestreamId">Internal <see cref="Livestream"/> id</param>
        /// <returns><see cref="LivestreamStartStreamingResponseModel"/></returns>
        [HttpPost("{livestreamId}/start_streaming")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> StartStreamingAsync([FromRoute]Guid livestreamId)
        {
            try
            {
                livestreamId.ThrowIfNullOrEmpty();

                var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
                var livestream = await _livestreamRepository.GetAsync(livestreamId).ConfigureAwait(false);

                // Await service 
                var vlog = await _userStreamingHandlingService.OnUserStartStreaming(user.Id, livestreamId).ConfigureAwait(false);

                // Commit and return
                return Ok(new LivestreamStartStreamingResponseModel { VlogId = vlog.Id });
            }
            catch (UserNotOwnerException e)
            {
                logger.LogError(e.Message);
                return Conflict(this.Error(ErrorCodes.InsufficientAccessRights, "The current user does not own this livestream."));
            }
            catch (LivestreamStateException e)
            {
                logger.LogError(e.Message);
                return Conflict(this.Error(ErrorCodes.InvalidOperation, "Livestream state is invalid for this operation"));
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return Conflict(this.Error(ErrorCodes.InvalidOperation, "Could not start user streaming process for livestream"));
            }
        }

        /// <summary>
        /// This gets called when a <see cref="SwabbrUser"/> is finished streaming
        /// on a specified <see cref="Livestream"/>. The vlog will always be published.
        /// </summary>
        /// <param name="livestreamId">Internal <see cref="Livestream"/> id</param>
        /// <returns><see cref="OkResult"/> or <see cref="ConflictResult"/></returns>
        [HttpPost("{livestreamId}/stop_streaming")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(VlogOutputModel))]
        public async Task<IActionResult> StopStreamingAsync([FromRoute]Guid livestreamId)
        {
            try
            {
                livestreamId.ThrowIfNullOrEmpty();

                var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
                var livestream = await _livestreamRepository.GetAsync(livestreamId).ConfigureAwait(false);

                // Await service
                await _userStreamingHandlingService.OnUserStopStreaming(user.Id, livestreamId).ConfigureAwait(false);

                return Ok();
            }
            catch (UserNotOwnerException e)
            {
                logger.LogError(e.Message);
                return Conflict(this.Error(ErrorCodes.InsufficientAccessRights, "The current user does not own this livestream."));
            }
            catch (LivestreamStateException e)
            {
                logger.LogError(e.Message);
                return Conflict(this.Error(ErrorCodes.InvalidOperation, "Livestream state is invalid for this operation"));
            }
            catch (Exception e)
            {
                // TODO If we reach this, the livestream might not be closed. How to handle this resource leak?
                logger.LogError(e.Message);
                return Conflict(this.Error(ErrorCodes.InvalidOperation, "Could not stop livestream"));
            }
        }

        [HttpPost("{livestreamId}/publish")]
        public Task<IActionResult> PublishLivestream([FromRoute]Guid livestreamId)
        {
            // TODO Public or private
            throw new NotImplementedException();
        }

    }

}
