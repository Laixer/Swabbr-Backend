using Laixer.Utility.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Swabbr.AzureFunctions.Parsing;
using Swabbr.AzureMediaServices.Utility;
using Swabbr.Core.Interfaces.Services;
using Swabbr.Core.Utility;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Swabbr.AzureFunctions.Functions
{
    /// <summary>
    /// Triggers when a <see cref="Core.Entities.Reaction"/> transcoding job failed.
    /// </summary>
    public sealed class ReactionTranscodedFailedHttpFunction
    {
        private readonly IReactionService _reactionService;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public ReactionTranscodedFailedHttpFunction(IReactionService reactionService)
        {
            _reactionService = reactionService ?? throw new ArgumentNullException(nameof(reactionService));
        }

        /// <summary>
        /// Triggers when a <see cref="Reaction"/> is transcoded in Azure Media Services.
        /// </summary>
        /// <param name="req"><see cref="HttpRequest"/></param>
        /// <param name="log"><see cref="ILogger"/></param>
        /// <returns><see cref="IActionResult"/></returns>
        [FunctionName(nameof(ReactionTranscodedFailedHttpFunction))]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req, ILogger log)
        {
            if (req == null) { throw new ArgumentNullException(nameof(req)); }
            if (log == null) { throw new ArgumentNullException(nameof(log)); }

            using var streamReader = new StreamReader(req.Body);
            var wrapper = JsonConvert.DeserializeObject<AMSEventGridJobResultMessage>(await streamReader.ReadToEndAsync().ConfigureAwait(false));
            if (wrapper.Subject.IsNullOrEmpty()) { return new BadRequestObjectResult($"Missing {nameof(AMSEventGridMessageBase.Subject)}"); }
            if (wrapper.EventTime.IsNullOrEmpty()) { return new BadRequestObjectResult($"Missing {nameof(AMSEventGridMessageBase.EventTime)}"); }
            if (wrapper.EventType.IsNullOrEmpty()) { return new BadRequestObjectResult($"Missing {nameof(AMSEventGridMessageBase.EventType)}"); }
            if (wrapper.Data.Outputs == null || !wrapper.Data.Outputs.Any()) { return new BadRequestObjectResult("Event grid data object doesn't contain any AMS outputs"); }
            if (wrapper.Data.Outputs.Count() > 1) { return new BadRequestObjectResult("Event grid data object contained more than one AMS output"); }

            // Extract the reaction id
            var reactionId = AMSAssetNameIdExtractor.GetId(wrapper.Data.Outputs.First().AssetName);

            // Log and process
            log.LogInformation($"Triggered {nameof(ReactionTranscodedFailedHttpFunction)} for reaction {reactionId} at { wrapper.EventTime}");
            await _reactionService.OnTranscodingReactionFailedAsync(reactionId).ConfigureAwait(false);
            log.LogInformation($"Finished {nameof(ReactionTranscodedFailedHttpFunction)} for reaction {reactionId} at { wrapper.EventTime}");

            return new OkResult();
        }
    }
}
