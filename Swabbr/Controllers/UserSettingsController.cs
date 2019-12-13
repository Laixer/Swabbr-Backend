using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swabbr.Api.ViewModels;
using Swabbr.Core.Entities;
using System.Net;
using System.Threading.Tasks;

namespace Swabbr.Api.Controllers
{
    /// <summary>
    /// Controller for handling requests related to user settings.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/v1/vlogs")]
    public class UserSettingsController : ControllerBase
    {
        /// <summary>
        /// Get user settings for the authenticated user.
        /// </summary>
        [HttpGet("get")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(UserSettingsOutputModel))]
        public async Task<IActionResult> Get()
        {
            //TODO Not implemented
            return Ok(MockData.MockRepository.RandomUserSettingsOutput());
        }

        /// <summary>
        /// Update user settings.
        /// </summary>
        [HttpPut("update")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(UserSettingsOutputModel))]
        public async Task<IActionResult> Update([FromBody] UserSettings settings)
        {
            //TODO Not implemented
            return Ok(MockData.MockRepository.RandomUserSettingsOutput());
        }
    }
}