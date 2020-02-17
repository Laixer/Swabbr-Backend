using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Net;

namespace Swabbr.Api.Controllers
{

    /// <summary>
    /// Contains utility functionality which is used throughout all our controllers.
    /// </summary>
    [Route("api/v1")]
    public class ApiControllerBase : ControllerBase
    {

        /// <summary>
        /// Creates an <see cref="ObjectResult"/> object that produces an <see
        /// cref="HttpStatusCode.Forbidden"/> response.
        /// </summary>
        /// <param name="value">The content value to format in the entity body.</param>
        /// <returns><see cref="ObjectResult"/></returns>
        protected ObjectResult Forbidden([ActionResultObjectValue] object value) => StatusCode((int)HttpStatusCode.Forbidden, value);

        /// <summary>
        /// Creates a <see cref="StatusCodeResult"/> object that produces an <see
        /// cref="HttpStatusCode.Forbidden"/> response.
        /// </summary>
        /// <returns><see cref="ObjectResult"/></returns>
        protected StatusCodeResult Forbidden() => StatusCode((int)HttpStatusCode.Forbidden);

    }

}
