using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Swabbr.AzureFunctions.Parsing;
using Swabbr.AzureMediaServices.Utility;
using Swabbr.Core.Interfaces.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Swabbr.AzureFunctions.Functions
{

    /// <summary>
    /// Triggers when a <see cref="Core.Entities.Reaction"/> transcoding job succeeded.
    /// </summary>
    public sealed class ReactionTranscodedSucceededFunction
    {

        private readonly IReactionService _reactionService;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public ReactionTranscodedSucceededFunction(IReactionService reactionService)
        {
            _reactionService = reactionService ?? throw new ArgumentNullException(nameof(reactionService));
        }

        /// <summary>
        /// Triggers when a <see cref="Reaction"/> is transcoded in Azure Media Services.
        /// </summary>
        /// <param name="eventGridEvent"><see cref="EventGridEvent"/></param>
        /// <param name="log"><see cref="ILogger"/></param>
        /// <returns><see cref="Task"/></returns>
        [FunctionName(nameof(ReactionTranscodedSucceededFunction))]
        public async Task Run([EventGridTrigger]EventGridEvent eventGridEvent, ILogger log)
        {
            if (eventGridEvent == null) { throw new ArgumentNullException(nameof(eventGridEvent)); }
            if (log == null) { throw new ArgumentNullException(nameof(log)); }

            // First extract the data
            var data = JsonConvert.DeserializeObject<AMSJobResultMessage>(eventGridEvent.Data.ToString());
            if (data.Outputs == null || !data.Outputs.Any()) { throw new ArgumentException("Event grid data object doesn't contain any AMS outputs"); }
            if (data.Outputs.Count() > 1) { throw new ArgumentException("Event grid data object contained more than one AMS output"); }

            // Extract the reaction id
            var reactionId = AMSNameGenerator.ReactionOutputAssetNameInverted(data.Outputs.First().AssetName);

            // Log and process
            log.LogInformation($"Triggered {nameof(ReactionTranscodedSucceededFunction)} for reaction {reactionId} at { eventGridEvent.EventTime}");
            await _reactionService.OnTranscodingReactionSucceededAsync(reactionId).ConfigureAwait(false);
            log.LogInformation($"Finished {nameof(ReactionTranscodedSucceededFunction)} for reaction {reactionId} at { eventGridEvent.EventTime}");
        }

    }

}
