using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swabbr.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Swabbr.Api.Controllers
{
    /// <summary>
    /// Controller for checking our resource health.
    /// </summary>
    [ApiController]
    [ApiVersion("1")]
    [Route("api/{version:apiVersion}/health")]
    public class HealthCheckController
    {
        private readonly IHealthCheckService _healthCheckService;
        private readonly ILogger logger;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        /// <param name="healthCheckService"><see cref="IUserSelectionService"/></param>
        /// <param name="loggerFactory"><see cref="ILoggerFactory"/></param>
        public HealthCheckController(IHealthCheckService healthCheckService,
            ILoggerFactory loggerFactory)
        {
            _healthCheckService = healthCheckService ?? throw new ArgumentNullException(nameof(healthCheckService));
            logger = (loggerFactory != null) ? loggerFactory.CreateLogger(nameof(HealthCheckController)) : throw new ArgumentNullException(nameof(loggerFactory));

        }

        /// <summary>
        /// Checks if our API is still healthy.
        /// </summary>
        /// <returns><see cref="IActionResult"/></returns>
        [AllowAnonymous]
        [HttpGet("check")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.ServiceUnavailable)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Check()
        {
            try
            {
                return ((await _healthCheckService.IsHealthyAsync())
                    ? new StatusCodeResult((int)HttpStatusCode.OK) : new StatusCodeResult((int)HttpStatusCode.ServiceUnavailable));
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
