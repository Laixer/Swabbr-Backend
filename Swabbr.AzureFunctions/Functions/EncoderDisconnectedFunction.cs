using Laixer.Utility.Extensions;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Swabbr.AzureFunctions.Utility;
using Swabbr.Core.Enums;
using Swabbr.Core.Interfaces.Services;
using System;
using System.Threading.Tasks;
using System.Transactions;

namespace Swabbr.AzureFunctions.Functions
{

    /// <summary>
    /// Azure Function triggering when a user connects to a live event.
    /// </summary>
    public sealed class EncoderDisconnectedFunction
    {

        private readonly IUserStreamingHandlingService _userStreamingHandlingService;
        private readonly ILivestreamService _livestreamService;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public EncoderDisconnectedFunction(IUserStreamingHandlingService userStreamingHandlingService,
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
        [FunctionName("EncoderDisconnectedFunction")]
        public async Task Run([EventGridTrigger]EventGridEvent eventGridEvent, ILogger log)
        {
            if (eventGridEvent == null) { throw new ArgumentNullException(nameof(eventGridEvent)); }
            if (log == null) { throw new ArgumentNullException(nameof(log)); }

            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            
            var livestream = await _livestreamService.GetLivestreamFromExternalIdAsync(EventGridEventUtility.ExtractLiveEventExternalId(eventGridEvent)).ConfigureAwait(false);

            // State race condition where the livestream is already decoupled from the user
            if (livestream.LivestreamState != LivestreamState.Live)
            {
                log.LogInformation($"No need to call {nameof(EncoderDisconnectedFunction)}, livestream not in {LivestreamState.Live.GetEnumMemberAttribute()} state");
                return;
            }

            // Log and process
            log.LogInformation($"Triggered {nameof(EncoderDisconnectedFunction)} at { eventGridEvent.EventTime}");
            await _userStreamingHandlingService.OnUserDisconnectedFromLivestreamAsync(livestream.UserId, livestream.Id).ConfigureAwait(false);
            log.LogInformation($"Finished {nameof(EncoderDisconnectedFunction)} for { eventGridEvent.EventTime}");
            scope.Complete();
        }

    }

}
