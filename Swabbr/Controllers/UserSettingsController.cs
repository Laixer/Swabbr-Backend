using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swabbr.Api.Authentication;
using Swabbr.Api.ViewModels;
using Swabbr.Core.Entities;
using Swabbr.Core.Interfaces;
using System.Net;
using System.Threading.Tasks;

namespace Swabbr.Api.Controllers
{
    /// <summary>
    /// Controller for handling requests related to user settings.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/v1/users/self/settings")]
    public class UserSettingsController : ControllerBase
    {
        private readonly IUserSettingsRepository _userSettingsRepository;
        private readonly UserManager<SwabbrIdentityUser> _userManager;

        public UserSettingsController(IUserSettingsRepository userSettingsRepository, UserManager<SwabbrIdentityUser> userManager)
        {
            _userSettingsRepository = userSettingsRepository;
            _userManager = userManager;
        }

        /// <summary>
        /// Get user settings for the authenticated user.
        /// </summary>
        [HttpGet("get")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(UserSettingsOutputModel))]
        public async Task<IActionResult> Get()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var uid = currentUser.UserId.ToString();
            var settings = _userSettingsRepository.RetrieveAsync(uid, uid);

            //TODO Not implemented
            return Ok(MockData.MockRepository.RandomUserSettingsOutput());
        }

        /// <summary>
        /// Update user settings.
        /// </summary>
        [HttpPut("update")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(UserSettingsOutputModel))]
        public async Task<IActionResult> Update([FromBody] UserSettingsInputModel input)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            //TODO Check if invalid userid has been given?
            input.UserId = currentUser.UserId;
            
            await _userSettingsRepository.UpdateAsync(input);

            // TODO Return updated settings (input model that was updated)

            //TODO Not implemented
            return Ok(MockData.MockRepository.RandomUserSettingsOutput());
        }
    }
}