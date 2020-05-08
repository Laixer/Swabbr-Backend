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
        private readonly ILivestreamPlaybackService _livestreamPlaybackService;
        private readonly ILivestreamRepository _livestreamRepository; // TODO Is this really the controllers job? (used for checks only)
        private readonly IUserStreamingHandlingService _userStreamingHandlingService;
        private readonly ILogger logger;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public LivestreamsController(UserManager<SwabbrIdentityUser> userManager,
            ILivestreamService livestreamingService,
            ILivestreamPlaybackService livestreamPlaybackService,
            ILivestreamRepository livestreamRepository,
            ILoggerFactory loggerFactory,
            IUserStreamingHandlingService userStreamingHandlingService)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _livestreamingService = livestreamingService ?? throw new ArgumentNullException(nameof(livestreamingService));
            _livestreamPlaybackService = livestreamPlaybackService ?? throw new ArgumentNullException(nameof(livestreamPlaybackService));
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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(LivestreamStartStreamingResponseModel))]
        [ProducesResponseType((int)HttpStatusCode.Conflict, Type = typeof(ErrorMessage))]
        public async Task<IActionResult> StartStreamingAsync([FromRoute]Guid livestreamId)
        {
            try
            {
                livestreamId.ThrowIfNullOrEmpty();

                var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);

                // Await service and return results
                var upstreamDetails = await _userStreamingHandlingService.OnUserStartStreaming(user.Id, livestreamId).ConfigureAwait(false);
                return Ok(new LivestreamStartStreamingResponseModel
                {
                    ApplicationName = upstreamDetails.ApplicationName,
                    HostPort = upstreamDetails.HostPort,
                    HostServer = upstreamDetails.HostServer,
                    Username = upstreamDetails.Username,
                    LivestreamId = upstreamDetails.LivestreamId,
                    Password = upstreamDetails.Password,
                    StreamKey = upstreamDetails.StreamKey,
                    VlogId = upstreamDetails.VlogId
                });
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
            catch (EntityNotFoundException e)
            {
                logger.LogError(e.Message);
                return Conflict(this.Error(ErrorCodes.EntityNotFound, "Livestream could not be found"));
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
        [ProducesResponseType((int)HttpStatusCode.Conflict, Type = typeof(ErrorMessage))]
        public async Task<IActionResult> StopStreamingAsync([FromRoute]Guid livestreamId)
        {
            return Conflict(this.Error(ErrorCodes.InvalidOperation, "There is no need to call this function anymore, it was done using events. This will be removed shortly."));

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

        /// <summary>
        /// Indicates that a user is going to start streaming to the given 
        /// <see cref="Livestream"/>.
        /// </summary>
        /// <param name="livestreamId">Internal <see cref="Livestream"/> id</param>
        /// <returns><see cref="LivestreamStartStreamingResponseModel"/></returns>
        [HttpGet("{livestreamId}/watch")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(LivestreamPlaybackOutputModel))]
        [ProducesResponseType((int)HttpStatusCode.Conflict, Type = typeof(ErrorMessage))]
        public async Task<IActionResult> RequestDownstreamDetailsAsync([FromRoute]Guid livestreamId)
        {
            try
            {
                livestreamId.ThrowIfNullOrEmpty();

                var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);

                var pars = await _livestreamPlaybackService.GetLivestreamDownstreamParametersAsync(livestreamId, user.Id).ConfigureAwait(false);
                return Ok(new LivestreamPlaybackOutputModel
                {
                    EndpointUrl = pars.EndpointUrl,
                    LiveLivestreamId = pars.LiveLivestreamId,
                    LiveUserId = pars.LiveUserId,
                    LiveVlogId = pars.LiveVlogId,
                    Token = pars.Token
                });
            }
            catch (LivestreamStateException e)
            {
                logger.LogError(e.Message);
                return Conflict(this.Error(ErrorCodes.InvalidOperation, "Livestream state is invalid for this operation"));
            }
            catch (EntityNotFoundException e)
            {
                logger.LogError(e.Message);
                return Conflict(this.Error(ErrorCodes.EntityNotFound, "Livestream could not be found"));
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return Conflict(this.Error(ErrorCodes.InvalidOperation, "Could not get downstream token for livestream"));
            }
        }

    }

}
