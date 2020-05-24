using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Swabbr.Api.Controllers
{

    /// <summary>
    /// Redirects us to the generated Swagger documentation for this Web API.
    /// </summary>
    [ApiVersion("1")]
    [Route("api/{version:apiVersion}/")]
    public class HomeController : ControllerBase
    {

        /// <summary>
        /// Redirects the homepage to the Swagger documentation.
        /// </summary>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("/")]
        public async Task<IActionResult> IndexAsync() => Redirect("/swagger");

    }

}
