using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swabbr.Api.Services;
using System;
using System.Threading.Tasks;
using Laixer.Utility.Extensions;
using Swabbr.Core.Interfaces.Services;
using Microsoft.AspNetCore.Identity;
using Swabbr.Api.Authentication;
using Swabbr.WowzaStreamingCloud.Services;

namespace Swabbr.Api.Controllers
{

    /// <summary>
    /// Debug functionality.
    /// TODO Indicate this debug functionality for the final product!
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("debug")]
    public class DebugController : ApiControllerBase
    {

        private readonly IVlogTriggerService _vlogTriggerService;
        private readonly UserManager<SwabbrIdentityUser> _userManager;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public DebugController(IVlogTriggerService vlogTriggerService,
            UserManager<SwabbrIdentityUser> userManager)
        {
            _vlogTriggerService = vlogTriggerService ?? throw new ArgumentNullException(nameof(vlogTriggerService));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        /// <summary>
        /// Debug function to launch our <see cref="VlogTriggerService"/>.
        /// </summary>
        /// <returns><see cref="OkResult"/></returns>
        [Authorize]
        [HttpPost("trigger_vlog")]
        public async Task<IActionResult> DriveVlogTriggerService(Guid userId)
        {
            try
            {
                userId.ThrowIfNullOrEmpty();

                //var user = await _userManager.GetUserAsync(User).ConfigureAwait(false);
                //if (user.Id != userId) { throw new InvalidOperationException("Not this user"); }

                await _vlogTriggerService.ProcessVlogTriggerForUserAsync(userId).ConfigureAwait(false);
                return Ok();
            } 
            catch (Exception e)
            {
                return Conflict();
            }
        }

        [HttpGet("token_test")]
        public IActionResult TokenTest(string sharedKey)
        {
            var token = WowzaAuthenticationService.GenerateTokenHmac(sharedKey);
            return Ok(token);
        }

    }
}
