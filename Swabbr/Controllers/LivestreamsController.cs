using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swabbr.Api.ViewModels;
using Swabbr.Core.Interfaces;
using Swabbr.Core.Notifications;
using System;
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
        private readonly ILivestreamRepository _livestreamRepository;

        public LivestreamsController(
            ILivestreamingService livestreamingService,
            ILivestreamRepository livestreamRepository,
            INotificationService notificationService)
        {
            _livestreamingService = livestreamingService;
            _livestreamRepository = livestreamRepository;
            _notificationService = notificationService;
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
        /// Start an available livestream
        /// </summary>
        [HttpGet("test/{userId}")]
        public async Task<IActionResult> NotifyUserStream(Guid userId)
        {
            // TODO Obtain available livestream from pool
            var connectionDetails = await _livestreamingService.GetAvailableStreamConnectionAsync();

            var notification = new SwabbrNotification
            {
                Content = new SwabbrMessage
                {
                }
            };

            await _notificationService.SendNotificationToUserAsync(notification, userId);

            // TODO Create notification with connection details as message content

            // TODO Send notification to user

            return NotFound("No");
        }

        /// <summary>
        /// Start an available livestream
        /// </summary>
        /// <param name="id">Id of the livestream</param>
        [HttpPut("start/{id}")]
        public async Task<IActionResult> StartStream([FromQuery] string id)
        {
            await _livestreamingService.StartStreamAsync(id);

            // TODO Get followers of authenticated user TODO For each follower, send notification

            return Ok();
        }

        /// <summary>
        /// Stop a running livestream
        /// </summary>
        [HttpPut("stop/{id}")]
        public async Task<IActionResult> StopStream([FromQuery] string id)
        {
            await _livestreamingService.StopStreamAsync(id);

            // TODO Set livestream with id {id} availability to true

            // TODO Should also happen automatically

            return Ok();
        }

        /// <summary>
        /// Returns playback details of a livestream.
        /// </summary>
        [HttpGet("playback/{id}")]
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
        public async Task<IActionResult> GetConnectionDetails([FromQuery] string id)
        {
            //TODO Check if user has permission to request these details

            var output = await _livestreamingService.GetStreamConnectionAsync(id);

            return Ok(new StreamConnectionDetailsOutputModel
            {
                Id = output.Id,
                AppName = output.AppName,
                HostAddress = output.HostAddress,
                Password = output.Password,
                Port = output.Port,
                StreamName = output.StreamName,
                Username = output.Username
            });
        }
    }
}