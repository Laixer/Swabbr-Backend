using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Swabbr.Api.Controllers
{
    public class DebugController : ApiControllerBase
    {

        public async Task<IActionResult> Get()
        {
            await Task.CompletedTask;
            return Ok("hello");
        }

    }
}
