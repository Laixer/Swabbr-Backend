using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Swabbr.Api.Controllers
{
    [ApiController]
    public class HomeController : ControllerBase
    {
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("/")]
        public async Task<IActionResult> Index()
        {
            return Redirect("/swagger");
        }
    }
}