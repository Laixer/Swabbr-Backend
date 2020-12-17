using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Swabbr.Core.Types;
using System.Net;

namespace Swabbr.Api.Controllers
{
    /// <summary>
    ///     Controller for handling error responses.
    /// </summary>
    [AllowAnonymous]
    public class ErrorController : ControllerBase
    {
        // GET: api/error
        /// <summary>
        ///     Returns a <see cref="ProblemDetails"/> from the http context.
        /// </summary>
        /// <param name="webHostEnvironment">Current webhost environment.</param>
        /// <param name="logger">Logger.</param>
        /// <returns><see cref="ProblemDetails"/>.</returns>
        [Route("error")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Error([FromServices] IWebHostEnvironment webHostEnvironment, [FromServices] ILogger<ErrorController> logger)
        {
            var error = HttpContext.Features.Get<ErrorMessage>();

            // If the error message is not set just return a generic problem.
            if (error is null)
            {
                logger.LogWarning($"Cannot return configured error message from exception, return generic problem");

                if (webHostEnvironment.IsDevelopment())
                {
                    logger.LogWarning("This should not be invoked in development environments.");
                }

                return Problem(
                    title: "Application was unable to process the request.",
                    statusCode: (int)HttpStatusCode.InternalServerError);
            }

            // Map the error message to a problem and return.
            return Problem(
                title: error.Message,
                statusCode: error.StatusCode);
        }
    }
}
