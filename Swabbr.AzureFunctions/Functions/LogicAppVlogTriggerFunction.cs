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
    /// Http trigger to trigger <see cref="IVlogTriggerService.ProcessVlogTriggerForUserAsync(Guid, DateTimeOffset)"/>.
    /// </summary>
    public sealed class LogicAppVlogTriggerFunction
    {

        private readonly IVlogTriggerService _vlogTriggerService;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public LogicAppVlogTriggerFunction(IVlogTriggerService vlogTriggerService)
        {
            _vlogTriggerService = vlogTriggerService ?? throw new ArgumentNullException(nameof(vlogTriggerService));
        }

        /// <summary>
        /// Triggers <see cref="IVlogTriggerService.ProcessVlogTriggerForUserAsync(Guid, DateTimeOffset)"/>.
        /// </summary>
        /// <param name="req"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName(nameof(LogicAppVlogTriggerFunction))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            var wrapper = JsonConvert.DeserializeObject<VlogTriggerWrapper>(await new StreamReader(req.Body).ReadToEndAsync());
            wrapper.UserId.ThrowIfNullOrEmpty();
            wrapper.UserTriggerMinute.ThrowIfNullOrEmpty();
            if (wrapper.VlogRequestTimeoutMinutes < 1) { throw new ArgumentOutOfRangeException(nameof(wrapper.VlogRequestTimeoutMinutes)); }

            log.LogInformation($@"{nameof(LogicAppVlogTriggerFunction)} - 
                Starting vlog trigger for user {wrapper.UserId} 
                trigger minute {wrapper.UserTriggerMinute}");

            await _vlogTriggerService.ProcessVlogTriggerForUserAsync(wrapper.UserId, wrapper.UserTriggerMinute).ConfigureAwait(false);

            log.LogInformation($@"{nameof(LogicAppVlogTriggerFunction)} - 
                Finished vlog trigger for user {wrapper.UserId} 
                trigger minute {wrapper.UserTriggerMinute}");

            return new OkResult();
        }

    }

}
