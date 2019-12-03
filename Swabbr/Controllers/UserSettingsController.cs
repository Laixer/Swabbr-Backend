using Microsoft.AspNetCore.Mvc;
using Swabbr.Core.Models;
using System;
using System.Threading.Tasks;

namespace Swabbr.Api.Controllers
{
    /// <summary>
    /// Actions related to user settings and preferences.
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UserSettingsController : ControllerBase
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
        public async Task<IActionResult> Update([FromBody] UserSettings settings)
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