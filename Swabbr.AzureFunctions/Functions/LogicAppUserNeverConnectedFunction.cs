using Laixer.Utility.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Swabbr.AzureFunctions.Types;
using Swabbr.Core.Interfaces.Services;
using Swabbr.Core.Utility;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Swabbr.AzureFunctions.Functions
{

    /// <summary>
    /// Occurs a given timeout period after the user said he was going live.
    /// </summary>
    public sealed class LogicAppUserNeverConnectedFunction
    {

        private readonly IUserStreamingHandlingService _userStreamingHandlingService;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public LogicAppUserNeverConnectedFunction(IUserStreamingHandlingService userStreamingHandlingService)
        {
            _userStreamingHandlingService = userStreamingHandlingService ?? throw new ArgumentNullException(nameof(userStreamingHandlingService));
        }

        /// <summary>
        /// Occurs a given timeout period after the user said he was going live.
        /// </summary>
        /// <param name="req"><see cref="HttpRequest"/></param>
        /// <param name="log"><see cref="ILogger"/></param>
        /// <returns><see cref="IActionResult"/></returns>
        [FunctionName(nameof(LogicAppUserNeverConnectedFunction))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            var wrapper = JsonConvert.DeserializeObject<UserNeverConnectedWrapper>(await new StreamReader(req.Body).ReadToEndAsync());
            wrapper.LivestreamId.ThrowIfNullOrEmpty();
            wrapper.UserId.ThrowIfNullOrEmpty();

            log.LogInformation($@"{nameof(LogicAppUserNeverConnectedFunction)} - 
                Starting user connect check for user {wrapper.UserId} 
                on livestream {wrapper.LivestreamId}");

            await _userStreamingHandlingService.OnUserNeverConnectedCheckAsync(wrapper.UserId, wrapper.LivestreamId).ConfigureAwait(false);

            log.LogInformation($@"{nameof(LogicAppUserNeverConnectedFunction)} - 
                Finished user connect check for user {wrapper.UserId} 
                on livestream {wrapper.LivestreamId}");

            return new OkResult();
        }

    }

}
