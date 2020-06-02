using Laixer.Utility.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Swabbr.AzureFunctions.Parsing;
using Swabbr.Core.Enums;
using Swabbr.Core.Interfaces.Services;
using Swabbr.Core.Utility;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Transactions;

namespace Swabbr.AzureFunctions.Functions
{
    /// <summary>
    /// Azure Function triggering when a user connects to a live event.
    /// This is the same as <see cref="EncoderDisconnectedFunction"/> but wrapped
    /// in a webhook.
    /// TODO DRY
    /// </summary>
    public sealed class EncoderDisconnectedHttpFunction
    {
        private readonly IUserStreamingHandlingService _userStreamingHandlingService;
        private readonly ILivestreamService _livestreamService;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public EncoderDisconnectedHttpFunction(IUserStreamingHandlingService userStreamingHandlingService,
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
        [FunctionName(nameof(EncoderDisconnectedHttpFunction))]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req, ILogger log)
        {
            if (req == null) { throw new ArgumentNullException(nameof(req)); }
            if (log == null) { throw new ArgumentNullException(nameof(log)); }

            using var streamReader = new StreamReader(req.Body);
            var wrapper = JsonConvert.DeserializeObject<AMSEventGridMessageBase>(await streamReader.ReadToEndAsync().ConfigureAwait(false));
            if (wrapper.Subject.IsNullOrEmpty()) { return new BadRequestObjectResult($"Missing {nameof(AMSEventGridMessageBase.Subject)}"); }
            if (wrapper.EventTime.IsNullOrEmpty()) { return new BadRequestObjectResult($"Missing {nameof(AMSEventGridMessageBase.EventTime)}"); }
            if (wrapper.EventType.IsNullOrEmpty()) { return new BadRequestObjectResult($"Missing {nameof(AMSEventGridMessageBase.EventType)}"); }

            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            // First extract the data
            var liveEventExternalId = wrapper.Subject.Split('/')[1]; // TODO Unsafe
            var livestream = await _livestreamService.GetLivestreamFromExternalIdAsync(liveEventExternalId).ConfigureAwait(false);

            // State race condition where the livestream is already decoupled from the user
            if (livestream.LivestreamState != LivestreamState.Live)
            {
                log.LogInformation($"No need to call {nameof(EncoderDisconnectedHttpFunction)}, livestream not in {LivestreamState.Live.GetEnumMemberAttribute()} state");
                return new OkResult();
            }

            // Log and process
            log.LogInformation($"Triggered {nameof(EncoderDisconnectedHttpFunction)} at { wrapper.EventTime}");
            await _userStreamingHandlingService.OnUserDisconnectedFromLivestreamAsync(livestream.UserId, livestream.Id).ConfigureAwait(false);
            log.LogInformation($"Finished {nameof(EncoderDisconnectedHttpFunction)} for { wrapper.EventTime}");
            scope.Complete();

            return new OkResult();
        }
    }
}
