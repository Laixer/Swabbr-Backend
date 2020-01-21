﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swabbr.Api.Authentication;
using Swabbr.Api.ViewModels;
using Swabbr.Core.Interfaces;
using Swabbr.Core.Notifications;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Swabbr.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/livestreams")]
    public class LivestreamsController : ControllerBase
    {
        private readonly ILivestreamingService _livestreamingService;
        private readonly INotificationService _notificationService;
        private readonly UserManager<SwabbrIdentityUser> _userManager;
        private readonly ILivestreamRepository _livestreamRepository;

        public LivestreamsController(
            ILivestreamingService livestreamingService,
            ILivestreamRepository livestreamRepository,
            INotificationService notificationService,
            UserManager<SwabbrIdentityUser> userManager)
        {
            _livestreamingService = livestreamingService;
            _livestreamRepository = livestreamRepository;
            _notificationService = notificationService;
            _userManager = userManager;
        }

        // TODO Remove
        ////[HttpPost("create")]
        ////public async Task<IActionResult> CreateStream()
        ////{
        ////    var output = await livestreamingService.CreateNewStreamAsync("testName");
        ////
        ////    return Ok(new StreamConnectionDetailsOutputModel
        ////    {
        ////        Id = output.Id,
        ////        AppName = output.AppName,
        ////        HostAddress = output.HostAddress,
        ////        Password = output.Password,
        ////        Port = output.Port,
        ////        StreamName = output.StreamName,
        ////        Username = output.Username
        ////    });
        ////}

        /// <summary>
        /// Open an available livestream for a user
        /// </summary>
        [HttpGet("trigger/{userId}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(StreamConnectionDetailsOutputModel))]
        public async Task<IActionResult> NotifyUserStream(Guid userId)
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
        /// Start an available livestream
        /// </summary>
        /// <param name="id">Id of the livestream</param>
        [HttpPut("start/{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> StartStream(string id)
        {
            var identityUser = await _userManager.GetUserAsync(User);

            await _livestreamingService.StartStreamAsync(id);

            // TODO Get followers of authenticated user TODO For each follower, send notification

            // TODO Get vlog

            return Ok();
        }

        /// <summary>
        /// Stop a running livestream
        /// </summary>
        [HttpPut("stop/{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> StopStream(string id)
        {
            await _livestreamingService.StopStreamAsync(id);

            // TODO Set livestream with id {id} availability to true

            // TODO Should also happen automatically

            return Ok();
        }

        /// <summary>
        /// Stop all running livestreams
        /// </summary>
        [HttpPut("test/stopall")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> TestStopAllStreams()
        {
            var active = await _livestreamRepository.GetActiveLivestreamsAsync();

            foreach(var a in active)
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
        public async Task<IActionResult> GetPlayback(string id)
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
        public async Task<IActionResult> GetPlaybackForUser(Guid userId)
        {
            // TODO UserId has to be linked to AVAILABLE livestream id
            throw new NotImplementedException();
        }

        /// <summary>
        /// Delete an existing livestream
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpGet("delete/{id}")]
        public async Task<IActionResult> DeleteStream(string id)
        {
            await _livestreamingService.DeleteStreamAsync(id);
            return Ok();
        }

        // TODO Remove
        [HttpGet("stream/{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(StreamConnectionDetailsOutputModel))]
        public async Task<IActionResult> GetConnectionDetails(string id)
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