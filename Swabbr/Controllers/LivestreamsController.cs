﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swabbr.Api.Authentication;
using Swabbr.Api.ViewModels;
using Swabbr.Core.Entities;
using Swabbr.Core.Enums;
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

        /// <summary>
        /// Open an available livestream for a user
        /// </summary>
        [HttpGet("trigger/{userId}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(StreamConnectionDetailsOutputModel))]
        public async Task<IActionResult> NotifyUserStreamAsync(Guid userId)
        {
            // TODO Obtain available livestream from pool
            var connection = await _livestreamingService.ReserveLiveStreamForUserAsync(userId);

            // TODO Create notification with connection details as message content

            // TODO Send notification containing connection details to user

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
        /// Start broadcasting to an available livestream
        /// </summary>
        /// <param name="id">Id of the livestream</param>
        [HttpPut("start/{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> StartBroadcastAsync(string id)
        {
            var identityUser = await _userManager.GetUserAsync(User);

            var livestream = await _livestreamRepository.GetByIdAsync(id);

            // Ensure the requested user owns this livestream
            if (!livestream.UserId.Equals(identityUser.UserId))
            {
                return BadRequest("User currently does not have access to this livestream.");
            }

            // TODO Create vlog for user.
            var createdVlog = await _vlogRepository.CreateAsync(new Vlog
            {
                VlogId = Guid.NewGuid(),
                UserId = identityUser.UserId,
                DateStarted = DateTime.Now,
            });


            // Obtain the nickname to use for the notification alert
            string nickname = identityUser.Nickname;

            // Obtain the followers of the authenticated user
            var followers = (await (_followRequestRepository.GetIncomingForUserAsync(identityUser.UserId)))
                .Where(fr => fr.Status == FollowRequestStatus.Accepted);

            // Construct notification
            var notification = new SwabbrNotification
            {
                MessageContent = new SwabbrMessage
                {
                    Title = $"{nickname} is livestreaming right now!",
                    Body = $"{nickname} has just gone live.",
                    ClickAction = ClickActions.FOLLOWED_PROFILE_LIVE
                }
            };

            // Send a notification to each follower
            foreach (FollowRequest fr in followers)
            {
                Guid followerId = fr.RequesterId;
                await _notificationService.SendNotificationToUserAsync(notification, followerId);
            }

            return Ok();
        }

        /// <summary>
        /// Stop a running livestream
        /// </summary>
        [HttpPut("stop/{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> StopStreamAsync(string id)
        {
            await _livestreamingService.StopStreamAsync(id);

            var x = await _livestreamRepository.GetByIdAsync(id);
            x.IsActive = false;
            await _livestreamRepository.UpdateAsync(x);

            return Ok();
        }

        /// <summary>
        /// Stop all running livestreams
        /// </summary>
        [HttpPut("test/stopall")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> TestStopAllStreamsAsync()
        {
            var active = await _livestreamRepository.GetActiveLivestreamsAsync();

            foreach (var a in active)
            {
                // stop all active streams
                _ = _livestreamingService.StopStreamAsync(a.Id);
                a.IsActive = false;
                a.UserId = Guid.Empty;
                await _livestreamRepository.UpdateAsync(a);
            }

            // TODO Set livestream with id {id} availability to true

            // TODO Should also happen automatically

            return Ok();
        }

        /// <summary>
        /// Returns playback details of a livestream.
        /// </summary>
        [HttpGet("playback/{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(StreamPlaybackOutputModel))]
        public async Task<IActionResult> GetPlaybackAsync(string id)
        {
            var details = await _livestreamingService.GetStreamPlaybackAsync(id);

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
            // TODO UserId has to be linked to AVAILABLE livestream id
            throw new NotImplementedException();
        }

        /// <summary>
        /// Delete an existing livestream
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpGet("delete/{id}")]
        public async Task<IActionResult> DeleteStreamAsync(string id)
        {
            await _livestreamingService.DeleteStreamAsync(id);
            return Ok();
        }

        // TODO Remove
        [HttpGet("stream/{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(StreamConnectionDetailsOutputModel))]
        public async Task<IActionResult> GetConnectionDetailsAsync(string id)
        {
            //TODO Check if user has permission to request these details
            var connection = await _livestreamingService.GetStreamConnectionAsync(id);

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