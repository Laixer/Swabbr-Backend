using Swabbr.Core.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Swabbr.AzureFunctions.Types;
using Swabbr.Core.Interfaces.Services;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Swabbr.AzureFunctions.Functions
{

    /// <summary>
    /// Azure Function triggering when a user connects to a live event.
    /// </summary>
    public sealed class LogicAppEncoderConnectedFunction
    {

        private readonly IUserStreamingHandlingService _userStreamingHandlingService;
        private readonly ILivestreamService _livestreamService;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public LogicAppEncoderConnectedFunction(IUserStreamingHandlingService userStreamingHandlingService,
            ILivestreamService livestreamService)
        {
            _userStreamingHandlingService = userStreamingHandlingService ?? throw new ArgumentNullException(nameof(userStreamingHandlingService));
            _livestreamService = livestreamService ?? throw new ArgumentNullException(nameof(livestreamService));
        }

        /// <summary>
        /// Trigger when a user connects to a Live Event.
        /// </summary>
        /// <param name="eventGridEvent"><see cref="EventGridEvent"/></param>
        /// <param name="log"><see cref="ILogger"/></param>
        /// <returns><see cref="Task"/></returns>
        [FunctionName(nameof(LogicAppEncoderConnectedFunction))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            if (req == null) { throw new ArgumentNullException(nameof(req)); }

            // First extract the data
            using var streamReader = new StreamReader(req.Body);
            var wrapper = JsonConvert.DeserializeObject<VlogTimeExpiredWrapper>(await streamReader.ReadToEndAsync().ConfigureAwait(false));
            wrapper.LivestreamExternalId.ThrowIfNullOrEmpty();

            // Parse if required
            if (wrapper.LivestreamExternalId.Contains('/', StringComparison.InvariantCulture))
            {
                var split = wrapper.LivestreamExternalId.Split('/');
                if (split.Length != 2) { throw new FormatException("Could not extract live event name from request body"); }
                wrapper.LivestreamExternalId = split[1];
            }

            // Log and process
            log.LogInformation($"Triggered {nameof(LogicAppEncoderConnectedFunction)}");

            var livestream = await _livestreamService.GetLivestreamFromExternalIdAsync(wrapper.LivestreamExternalId).ConfigureAwait(false);
            await _userStreamingHandlingService.OnUserConnectedToLivestreamAsync(livestream.UserId, livestream.Id).ConfigureAwait(false);

            log.LogInformation($"Finished {nameof(LogicAppEncoderConnectedFunction)}");

            return new OkResult();
        }

    }

}
