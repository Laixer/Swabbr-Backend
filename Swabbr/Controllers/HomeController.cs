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
    }
}