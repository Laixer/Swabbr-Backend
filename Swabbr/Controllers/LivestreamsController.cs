using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Swabbr.Api.Controllers
{
    // Controller for handling requests related to livestreaming functions. TODO Using this
    // controller for now instead of Vlogs. Determine where to store these methods.
    [Obsolete("Livestreaming has not been implemented yet")]
    [ApiController]
    [Route("api/v1/livestreams")]
    public class LivestreamsController : ControllerBase
    {
        /// <summary>
        /// Start a new livestream.
        /// </summary>
        [HttpPost("start")]
        public async Task<IActionResult> Start()
        {
            //! TODO
            throw new NotImplementedException();
        }
    }
}