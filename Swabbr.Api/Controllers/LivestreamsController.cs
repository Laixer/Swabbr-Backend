using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Swabbr.Api.Authentication;
using Swabbr.Api.Errors;
using Swabbr.Api.Extensions;
using Swabbr.Api.ViewModels;
using Swabbr.Core.Entities;
using Swabbr.Core.Enums;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Interfaces;
using Swabbr.Core.Notifications;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Swabbr.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/livestreams")]
    public class LivestreamsController : ControllerBase
    {
        private readonly UserManager<SwabbrIdentityUser> _userManager;

        private readonly ILivestreamingService _livestreamingService;
        private readonly INotificationService _notificationService;

        private readonly ILivestreamRepository _livestreamRepository;
        private readonly IFollowRequestRepository _followRequestRepository;
        private readonly IVlogRepository _vlogRepository;

        public LivestreamsController(
            UserManager<SwabbrIdentityUser> userManager,
            ILivestreamingService livestreamingService,
            INotificationService notificationService,
            ILivestreamRepository livestreamRepository,
            IFollowRequestRepository followRequestRepository,
            IVlogRepository vlogRepository)
        {
            _userManager = userManager;
            _livestreamingService = livestreamingService;
            _notificationService = notificationService;
            _livestreamRepository = livestreamRepository;
            _followRequestRepository = followRequestRepository;
            _vlogRepository = vlogRepository;
        }

        // TODO The method below is limited and to be used TEMPORARILY for testing purposes only. It
        // should be removed.
        /// <summary>
        /// Open an available livestream for a user
        /// </summary>
        [Obsolete]
        [HttpGet("test/trigger/{userId}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(StreamConnectionDetailsOutputModel))]
        public async Task<IActionResult> NotifyUserStreamAsync(Guid userId)
        {
            var connection = await _livestreamingService.ReserveLiveStreamForUserAsync(userId);

            await _livestreamingService.StartStreamAsync(connection.Id);

            return Ok(new StreamConnectionDetailsOutputModel
            {
                Id = connection.Id,
                AppName = connection.AppName,
                HostAddress = connection.HostAddress,
                Password = connection.Password,
                Port = connection.Port,
                StreamName = connection.StreamName,
                Username = connection.Username
            });
        }

        /// <summary>
        /// Start the broadcasting to an available livestream.
        /// </summary>
        /// <param name="livestreamId">Id of the livestream</param>
        [HttpPut("{livestreamId}/startbroadcast")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> StartBroadcastAsync(string livestreamId)
        {
            var identityUser = await _userManager.GetUserAsync(User);

            var livestream = await _livestreamRepository.GetByIdAsync(livestreamId);

            // Ensure the requested user owns this livestream
            if (!livestream.UserId.Equals(identityUser.UserId))
            {
                return BadRequest(
                    this.Error(ErrorCodes.INSUFFICIENT_ACCESS_RIGHTS, "User currently does not have access to this livestream.")
                    );
            }
            if (await _vlogRepository.ExistsAsync(livestream.VlogId))
            {
                // If a vlog exists the livestream has already been started.
                return BadRequest(
                    this.Error(ErrorCodes.INVALID_INPUT, "This livestream has already been started.")
                    );
            }

            // Create a new vlog for the livestream
            var newVlog = await _vlogRepository.CreateAsync(new Vlog
            {
                VlogId = Guid.NewGuid(),
                LivestreamId = livestream.Id,
                UserId = identityUser.UserId,
                DateStarted = DateTime.Now,
                IsLive = true
            });

            // Bind the vlog to the livestream
            livestream.VlogId = newVlog.VlogId;
            await _livestreamRepository.UpdateAsync(livestream);

            // Obtain the nickname to use for the notification alert
            string nickname = identityUser.Nickname;

            // Obtain the followers of the authenticated user
            var followers = (await (_followRequestRepository.GetIncomingForUserAsync(identityUser.UserId)))
                .Where(fr => fr.Status == FollowRequestStatus.Accepted);

            // Construct notification
            var notification = new SwabbrNotification
            {
                MessageContent = new SwabbrNotificationBody
                {
                    Title = $"{nickname} is livestreaming right now!",
                    Body = $"{nickname} has just gone live.",
                    ClickAction = ClickActions.FOLLOWED_PROFILE_LIVE,
                    Object = JObject.FromObject(newVlog),
                    ObjectType = typeof(Vlog).Name
                }
            };

            // Send the notification to each follower
            foreach (FollowRequest fr in followers)
            {
                Guid followerId = fr.RequesterId;
                await _notificationService.SendNotificationToUserAsync(notification, followerId);
            }

            return Ok();
        }

        /// <summary>
        /// Publish a livestream as a vlog.
        /// </summary>
        [HttpPut("{livestreamId}/publish")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(VlogOutputModel))]
        public async Task<IActionResult> PublishAsync([FromRoute]string livestreamId, [FromBody]VlogUpdateModel input)
        {
            //TODO Check if vlog has already been published

            var identityUser = await _userManager.GetUserAsync(User);

            var livestream = await _livestreamRepository.GetByIdAsync(livestreamId);

            if (!livestream.UserId.Equals(identityUser.UserId))
            {
                return StatusCode(
                    (int)HttpStatusCode.Forbidden,
                    this.Error(ErrorCodes.INSUFFICIENT_ACCESS_RIGHTS, "User is not allowed to perform this action.")
                );
            }
            if (!(await _vlogRepository.ExistsAsync(livestream.VlogId)))
            {
                //TODO Determine what to do at this point
                // Failsave for if a vlog has not been created (livestream is being published without having been started)
                // Create a new vlog record
                var newVlog = await _vlogRepository.CreateAsync(new Vlog
                {
                    VlogId = Guid.NewGuid(),
                    LivestreamId = livestream.Id,
                    UserId = identityUser.UserId,
                    DateStarted = DateTime.Now
                });

                // Bind the vlog to the livestream
                livestream.VlogId = newVlog.VlogId;
                // Update livestream instance
                livestream = await _livestreamRepository.UpdateAsync(livestream);
            }

            // Update the livestream bound vlog
            var vlog = await _vlogRepository.GetByIdAsync(livestream.VlogId);
            vlog.IsPrivate = input.IsPrivate;
            VlogOutputModel output = await _vlogRepository.UpdateAsync(vlog);

            // Stop the external livestream asynchronously
            await _livestreamingService.StopStreamAsync(livestreamId);

            // Synchronize the livestream recordings for the vlog without awaiting result
            _ = _livestreamingService.SyncRecordingsForVlogAsync(livestreamId, livestream.VlogId);

            return Ok(output);
        }

        /// <summary>
        /// Get thumbnail of livestream.
        /// </summary>
        [HttpGet("{livestreamId}/thumbnail")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetThumbnailAsync(string livestreamId)
        {
            try
            {
                // Retrieve the live thumbnail of the livestream
                var thumbnailUrl = await _livestreamingService.GetThumbnailUrlAsync(livestreamId);
                return Ok(thumbnailUrl);
            }
            catch
            {
                //TODO: Handle specific exception
                return BadRequest(
                    this.Error(ErrorCodes.EXTERNAL_ERROR, "Could not retrieve thumbnail.")
                );
            }
        }

        /// <summary>
        /// Stop all running livestreams.
        /// </summary>
        [HttpPut("test/stopall")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> TestStopAllStreamsAsync()
        {
            //TODO: This method has been added for testing purposes only.
            var active = await _livestreamRepository.GetActiveLivestreamsAsync();

            foreach (var a in active)
            {
                // Currently not being awaited

                // Stop all active streams
                _ = _livestreamingService.StopStreamAsync(a.Id);
                a.IsActive = false;
                a.UserId = Guid.Empty;
                await _livestreamRepository.UpdateAsync(a);
            }

            return Ok();
        }

        /// <summary>
        /// Returns playback details of a livestream.
        /// </summary>
        [HttpGet("{livestreamId}/playback")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(StreamPlaybackOutputModel))]
        public async Task<IActionResult> GetPlaybackAsync(string livestreamId)
        {
            var details = await _livestreamingService.GetStreamPlaybackAsync(livestreamId);

            return Ok(new StreamPlaybackOutputModel
            {
                PlaybackUrl = details.PlaybackUrl
            });
        }

        /// <summary>
        /// Returns playback details of a livestream that is broadcasted by the specified user.
        /// </summary>
        [HttpGet("playback/user/{userId}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(StreamPlaybackOutputModel))]
        public async Task<IActionResult> GetPlaybackForUserAsync(Guid userId)
        {
            try
            {
                var activeStream = await _livestreamRepository.GetActiveLivestreamForUserAsync(userId);

                return Ok(new StreamPlaybackOutputModel
                {
                    PlaybackUrl = (await _livestreamingService.GetStreamPlaybackAsync(activeStream.Id)).PlaybackUrl
                });
            }
            catch (EntityNotFoundException)
            {
                return NotFound(
                    this.Error(ErrorCodes.ENTITY_NOT_FOUND, "Livestream for the specified user was not found.")
                    );
            }
        }

        /// <summary>
        /// Delete a Wowza Streaming Cloud livestream
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpGet("{livestreamId}/delete")]
        public async Task<IActionResult> DeleteStreamAsync(string livestreamId)
        {
            await _livestreamingService.DeleteStreamAsync(livestreamId);
            return Ok();
        }

        /// <summary>
        /// Retrieve the connection details of a livestream.
        /// </summary>
        [HttpGet("{livestreamId}/connection")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(StreamConnectionDetailsOutputModel))]
        public async Task<IActionResult> GetConnectionDetailsAsync(string livestreamId)
        {
            var identityUser = await _userManager.GetUserAsync(User);

            var livestream = await _livestreamRepository.GetByIdAsync(livestreamId);

            if (!livestream.UserId.Equals(identityUser.UserId))
            {
                return StatusCode(
                    (int)HttpStatusCode.Forbidden,
                    this.Error(ErrorCodes.INSUFFICIENT_ACCESS_RIGHTS, "User is not allowed to access livestream details.")
                );
            }

            var connection = await _livestreamingService.GetStreamConnectionAsync(livestreamId);

            return Ok(new StreamConnectionDetailsOutputModel
            {
                Id = connection.Id,
                AppName = connection.AppName,
                HostAddress = connection.HostAddress,
                Password = connection.Password,
                Port = connection.Port,
                StreamName = connection.StreamName,
                Username = connection.Username
            });
        }
    }
}