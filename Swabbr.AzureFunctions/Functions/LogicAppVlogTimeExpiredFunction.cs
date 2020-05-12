using Laixer.Utility.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Swabbr.AzureFunctions.Types;
using Swabbr.Core.Enums;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Interfaces.Services;
using Swabbr.Core.Utility;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Transactions;

namespace Swabbr.AzureFunctions.Functions
{

    /// <summary>
    /// Http trigger that fires after the user has livestreamed for a given
    /// maximum amount of minutes.
    /// </summary>
    public sealed class LogicAppVlogTimeExpiredFunction
    {

        private readonly IUserStreamingHandlingService _userStreamingHandlingService;
        private readonly ILivestreamService _livestreamService;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public LogicAppVlogTimeExpiredFunction(IUserStreamingHandlingService userStreamingHandlingService,
            ILivestreamService livestreamService)
        {
            _userStreamingHandlingService = userStreamingHandlingService ?? throw new ArgumentNullException(nameof(userStreamingHandlingService));
            _livestreamService = livestreamService ?? throw new ArgumentNullException(nameof(livestreamService));
        }

        /// <summary>
        /// Called when the users vlogtime has expired.
        /// </summary>
        /// <param name="req"><see cref="HttpRequest"/></param>
        /// <param name="log"><see cref="ILogger"/></param>
        /// <returns><see cref="IActionResult"/></returns>
        [FunctionName(nameof(LogicAppVlogTimeExpiredFunction))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            var wrapper = JsonConvert.DeserializeObject<VlogTimeExpiredWrapper>(await new StreamReader(req.Body).ReadToEndAsync());
            wrapper.LivestreamExternalId.ThrowIfNullOrEmpty();

            try
            {
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    // Parse if required
                    if (wrapper.LivestreamExternalId.Contains('/'))
                    {
                        wrapper.LivestreamExternalId = wrapper.LivestreamExternalId.Split('/')[1]; // TODO Unsafe
                    }

                    var livestream = await _livestreamService.GetLivestreamFromExternalIdAsync(wrapper.LivestreamExternalId).ConfigureAwait(false);

                    // This prevents a race condition where the livestream is already marked as another state
                    // This will remove the user id from the livestream in the data store
                    if (livestream.LivestreamStatus != LivestreamStatus.Live)
                    {
                        log.LogInformation($@"{nameof(LogicAppVlogTimeExpiredFunction)} - 
                            No need for processing vlogging time expired event for livestream {livestream.Id},
                            livestream is not in state {LivestreamStatus.Live.GetEnumMemberAttribute()}");
                        return new NoContentResult();
                    }

                    log.LogInformation($@"{nameof(LogicAppVlogTimeExpiredFunction)} - 
                        Processing vlogging time expired event for user {livestream.UserId} 
                        on livestream {livestream.Id}");

                    await _userStreamingHandlingService.OnUserVlogTimeExpiredAsync(livestream.UserId, livestream.Id).ConfigureAwait(false);

                    log.LogInformation($@"{nameof(LogicAppVlogTimeExpiredFunction)} - 
                        Finished processing vlogging time expired event for user {livestream.UserId} 
                        on livestream {livestream.Id}");

                    scope.Complete();
                }

                return new OkResult();
            }
            catch (EntityNotFoundException)
            {
                log.LogError($"Could not find livestream with external id {wrapper.LivestreamExternalId}");
                return new ConflictObjectResult($"Could not find livestream with external id {wrapper.LivestreamExternalId}");
            }
        }

    }

}
