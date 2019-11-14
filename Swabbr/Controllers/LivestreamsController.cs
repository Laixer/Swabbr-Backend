using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Swabbr.Controllers
{
    //! Handles livestreaming with Azure Media Services
    // TODO Using this controller for now instead of Vlogs. Determine where to store these methods.
    [ApiController]
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