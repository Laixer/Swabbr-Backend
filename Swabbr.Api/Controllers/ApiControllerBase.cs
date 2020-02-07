using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Net;

namespace Swabbr.Api.Controllers
{
    [Route("api/v1")]
    public class ApiControllerBase : ControllerBase
    {
        /// <summary>
        /// Creates an <see cref="ObjectResult"/> object that produces an <see
        /// cref="HttpStatusCode.Forbidden"/> response.
        /// </summary>
        /// <param name="value">The content value to format in the entity body.</param>
        protected ObjectResult Forbidden([ActionResultObjectValue] object value)
        {
            return StatusCode((int)HttpStatusCode.Forbidden, value);
        }

        /// <summary>
        /// Creates a <see cref="StatusCodeResult"/> object that produces an <see
        /// cref="HttpStatusCode.Forbidden"/> response.
        /// </summary>
        protected StatusCodeResult Forbidden()
        {
            return StatusCode((int)HttpStatusCode.Forbidden);
        }
    }
}