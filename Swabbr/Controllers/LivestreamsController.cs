﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Swabbr.Api.Controllers
{
    //! Handles livestreaming with Azure Media Services
    // TODO Using this controller for now instead of Vlogs. Determine where to store these methods.
    [Obsolete]
    [ApiController]
    [Route("api/v1/[controller]")]
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