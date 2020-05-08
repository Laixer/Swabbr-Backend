using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Swabbr.AzureFunctions.Types;
using Laixer.Utility.Extensions;
using Swabbr.Core.Utility;
using Swabbr.Core.Interfaces.Services;

namespace Swabbr.AzureFunctions.Functions
{

    /// <summary>
    /// Http trigger to trigger <see cref="IVlogTriggerService.ProcessVlogTimeoutForUserAsync(Guid, DateTimeOffset)"/>.
    /// </summary>
    public sealed class LogicAppVlogTimeoutFunction
    {

        private readonly IVlogTriggerService _vlogTriggerService;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public LogicAppVlogTimeoutFunction(IVlogTriggerService vlogTriggerService)
        {
            _vlogTriggerService = vlogTriggerService ?? throw new ArgumentNullException(nameof(vlogTriggerService));
        }

        /// <summary>
        /// Triggers <see cref="IVlogTriggerService.ProcessVlogTimeoutForUserAsync(Guid, DateTimeOffset)"/>.
        /// </summary>
        /// <param name="req"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName(nameof(LogicAppVlogTimeoutFunction))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            var wrapper = JsonConvert.DeserializeObject<VlogTriggerWrapper>(await new StreamReader(req.Body).ReadToEndAsync());
            wrapper.UserId.ThrowIfNullOrEmpty();
            wrapper.UserTriggerMinute.ThrowIfNullOrEmpty();
            if (wrapper.VlogRequestTimeoutMinutes < 1) { throw new ArgumentOutOfRangeException(nameof(wrapper.VlogRequestTimeoutMinutes)); }

            log.LogInformation($@"{nameof(LogicAppVlogTimeoutFunction)} - 
                Starting vlog timeout for user {wrapper.UserId} 
                trigger minute {wrapper.UserTriggerMinute}");

            await _vlogTriggerService.ProcessVlogTimeoutForUserAsync(wrapper.UserId, wrapper.UserTriggerMinute).ConfigureAwait(false);

            log.LogInformation($@"{nameof(LogicAppVlogTimeoutFunction)} - 
                Finished vlog timeout for user {wrapper.UserId} 
                trigger minute {wrapper.UserTriggerMinute}");

            return new OkResult();
        }

    }

}
