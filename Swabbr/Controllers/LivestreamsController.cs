using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swabbr.Api.ViewModels;
using Swabbr.Core.Interfaces;
using System;
using System.Threading.Tasks;

namespace Swabbr.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/livestreams")]
    public class LivestreamsController : ControllerBase
    {
        private readonly ILivestreamingService livestreamingService;

        public LivestreamsController(ILivestreamingService livestreamingService)
        {
            this.livestreamingService = livestreamingService;
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
        /// <param name="id">Id of the livestream</param>
        [HttpGet("start/{id}")]
        public async Task<IActionResult> StartStream([FromQuery] string id)
        {
            await livestreamingService.StartStreamAsync(id);

            // TODO Get followers of authenticated user
            // TODO For each follower, send notification

            return Ok();
        }

        /// <summary>
        /// Stop a running livestream
        /// </summary>
        [HttpGet("stop/{id}")]
        public async Task<IActionResult> StopStream([FromQuery] string id)
        {
            await livestreamingService.StopStreamAsync(id);

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
            var details = await livestreamingService.GetStreamPlaybackAsync(id);

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
            // TODO
            throw new NotImplementedException();
        }

        /// <summary>
        /// Delete an existing livestream
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpGet("delete/{id}")]
        public async Task<IActionResult> DeleteStream(string id)
        {
            await livestreamingService.DeleteStreamAsync(id);
            return Ok();
        }

        // TODO Remove
        [HttpGet("stream/{id}")]
        public async Task<IActionResult> GetConnectionDetails([FromQuery] string id)
        {
            //TODO Check if user has permission to request these details

            var output = await livestreamingService.GetStreamConnectionAsync(id);

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