using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Swabbr.Core.Models;

namespace Swabbr.Controllers
{
    /// <summary>
    /// Actions related to user settings and preferences.
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class SettingsController : ControllerBase
    {
        /// <summary>
        /// Get user settings for the authenticated user.
        /// </summary>
        [HttpGet("get")]
        public async Task<IActionResult> Get()
        {
            //! TODO
            throw new NotImplementedException();
        }

        /// <summary>
        /// Update user settings.
        /// </summary>
        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody] Settings settings)
        {
            if (settings != null)
            {
                return new OkResult();
            }

            //! TODO
            throw new NotImplementedException();
        }
    }
}