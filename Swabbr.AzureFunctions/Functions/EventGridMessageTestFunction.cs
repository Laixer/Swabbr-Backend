using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Swabbr.AzureFunctions.Functions
{
    /// <summary>
    /// TODO Remove
    /// </summary>
    public sealed class EventGridMessageTestFunction
    {
        /// <summary>
        /// Trigger when a user connects to a Live Event.
        /// </summary>
        /// <param name="req"><see cref="HttpRequest"/></param>
        /// <param name="log"><see cref="ILogger"/></param>
        /// <returns><see cref="Task"/></returns>
        [FunctionName(nameof(EventGridMessageTestFunction))]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req, ILogger log)
        {
            if (req == null) { throw new ArgumentNullException(nameof(req)); }
            if (log == null) { throw new ArgumentNullException(nameof(log)); }

            using var streamReader = new StreamReader(req.Body);
            var body = await streamReader.ReadToEndAsync().ConfigureAwait(false);

            log.LogInformation($"Body:\n{body}");

            return new OkObjectResult(body);
        }
    }
}
