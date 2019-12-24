using Microsoft.AspNetCore.Mvc;
using Swabbr.Api.ViewModels;
using Swabbr.Core.Interfaces;
using System;
using System.Threading.Tasks;

namespace Swabbr.Api.Controllers
{
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
        [HttpGet("/createstream")]
        public async Task<IActionResult> TestStream()
        {
            var output = await livestreamingService.CreateNewStreamAsync("testName");

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

        // TODO Remove
        [HttpGet("/startstream/{id}")]
        public async Task<IActionResult> StartStream([FromQuery] string id)
        {
            await livestreamingService.StartStreamAsync(id);

            return Ok();
        }

        // TODO Remove
        [HttpGet("/stop/{id}")]
        public async Task<IActionResult> StopStream([FromQuery] string id)
        {
            await livestreamingService.StopStreamAsync(id);

            return Ok();
        }

        [HttpGet("/get/stream/terribleurl/playback/{id}")]
        public async Task<IActionResult> GetPlayback(string id)
        {
            var details = await livestreamingService.GetStreamPlaybackAsync(id);

            return Ok(new StreamPlaybackOutputModel
            {
                PlaybackUrl = details.PlaybackUrl
            });
        }

        [HttpGet("/deletes/stream/terribleurl/playback/{id}")]
        public async Task<IActionResult> DePlayback(string id)
        {
            await livestreamingService.DeleteStreamAsync(id);

            return Ok("KK☻");
        }

        [HttpGet("/mulktipledelete/stream/terribleurl/playback/{id}")]
        public async Task<IActionResult> DeleteMany(string[] id)
        {
            foreach(string i in id)
            {
                await livestreamingService.DeleteStreamAsync(i);
            }

            return Ok("200/OK☻•◘○");
        }

        // TODO Remove
        [HttpGet("/getstream/{id}")]
        public async Task<IActionResult> GetAStream([FromQuery] string id)
        {
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