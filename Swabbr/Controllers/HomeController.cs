using Microsoft.AspNetCore.Mvc;
using Swabbr.Api.ViewModels;
using Swabbr.Core.Interfaces;
using System.Threading.Tasks;

namespace Swabbr.Api.Controllers
{
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly ILivestreamingService livestreamingService;

        public HomeController(ILivestreamingService livestreamingService)
        {
            this.livestreamingService = livestreamingService;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("/")]
        public async Task<IActionResult> Index()
        {
            return Redirect("/swagger");
        }

        // TODO Remove
        [HttpGet("/createstream")]
        public async Task<IActionResult> TestStream()
        {
            var output = await livestreamingService.CreateNewStreamAsync("testName");

            return Ok(new StreamConnectionDetailsOutput
            {
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

        // TODO Remove
        [HttpGet("/getstream/{id}")]
        public async Task<IActionResult> IdkStream([FromQuery] string id)
        {
            var output = await livestreamingService.GetStreamAsync(id);

            return Ok(new StreamConnectionDetailsOutput
            {
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