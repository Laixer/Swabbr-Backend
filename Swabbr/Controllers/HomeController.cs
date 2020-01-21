using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Swabbr.Api.Controllers
{
    public class HomeController : ControllerBase
    {
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("/")]
        public async Task<IActionResult> IndexAsync()
        {
            // Redirect the index page ("/") to the Swagger API definition.
            return Redirect("/swagger");
        }
    }
}