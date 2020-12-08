using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swabbr.Core.Interfaces.Services;
using System;
using System.Threading.Tasks;

namespace Swabbr.Api.Controllers
{
    /// <summary>
    ///     Controller for checking our resource health.
    /// </summary>
    [AllowAnonymous]
    [Route("health")]
    public class HealthCheckController : ControllerBase
    {
        private readonly IHealthCheckService _healthCheckService;

        /// <summary>
        ///     Create new instance.
        /// </summary>
        public HealthCheckController(IHealthCheckService healthCheckService) 
            => _healthCheckService = healthCheckService ?? throw new ArgumentNullException(nameof(healthCheckService));

        /// <summary>
        ///     Checks if our API is still healthy.
        /// </summary>
        [AllowAnonymous]
        [HttpGet("check")]
        public async Task<IActionResult> Check()
        {
            // Act.
            await _healthCheckService.IsHealthyAsync();

            // Return.
            return Ok();
        }
    }
}
