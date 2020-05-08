using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Swabbr.Core.Interfaces.Services;
using System;
using System.Threading.Tasks;

namespace Swabbr.AzureFunctions.Functions
{

    /// <summary>
    /// Azure Function triggering when a user connects to a live event.
    /// </summary>
    public sealed class EncoderConnectedFunction
    {

        private readonly IUserStreamingHandlingService _userStreamingHandlingService;
        private readonly ILivestreamService _livestreamService;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public EncoderConnectedFunction(IUserStreamingHandlingService userStreamingHandlingService,
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
        [FunctionName("EncoderConnectedFunction")]
        public async Task Run([EventGridTrigger]EventGridEvent eventGridEvent, ILogger log)
        {
            // First extract the data
            var liveEventExternalId = eventGridEvent.Subject.Split('/')[1]; // TODO Unsafe
            var livestream = await _livestreamService.GetLivestreamFromExternalIdAsync(liveEventExternalId).ConfigureAwait(false);

            // Log and process
            log.LogInformation($"Triggered {nameof(EncoderConnectedFunction)} at { eventGridEvent.EventTime}");
            await _userStreamingHandlingService.OnUserConnectedToLivestreamAsync(livestream.UserId, livestream.Id);
            log.LogInformation($"Finished {nameof(EncoderConnectedFunction)} for { eventGridEvent.EventTime}");
        }

    }

}
