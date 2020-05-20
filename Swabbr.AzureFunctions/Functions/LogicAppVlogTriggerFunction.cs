using Laixer.Utility.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Swabbr.AzureFunctions.Types;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Interfaces.Services;
using Swabbr.Core.Utility;
using System;
using System.IO;
using System.Threading.Tasks;

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
            if (req == null) { throw new ArgumentNullException(nameof(req)); }

            using var streamReader = new StreamReader(req.Body);
            var wrapper = JsonConvert.DeserializeObject<VlogTriggerWrapper>(await streamReader.ReadToEndAsync().ConfigureAwait(false));
            wrapper.UserId.ThrowIfNullOrEmpty();
            wrapper.UserTriggerMinute.ThrowIfNullOrEmpty();
            if (wrapper.VlogRequestTimeoutMinutes < 1) { throw new ArgumentOutOfRangeException(nameof(wrapper.VlogRequestTimeoutMinutes)); }

            log.LogInformation($@"{nameof(LogicAppVlogTriggerFunction)} - 
                Starting vlog trigger for user {wrapper.UserId} 
                trigger minute {wrapper.UserTriggerMinute}");
            try
            {
                await _vlogTriggerService.ProcessVlogTriggerForUserAsync(wrapper.UserId, wrapper.UserTriggerMinute).ConfigureAwait(false);
            }
            catch (UserAlreadyInLivestreamCycleException)
            {
                log.LogError($@"{nameof(LogicAppVlogTimeoutFunction)} -
                    User {wrapper.UserId} is already in a livestream cycle,
                    aborting request and returning Conflict");
                return new ConflictObjectResult(nameof(UserAlreadyInLivestreamCycleException));
            }

            log.LogInformation($@"{nameof(LogicAppVlogTriggerFunction)} - 
                Finished vlog trigger for user {wrapper.UserId} 
                trigger minute {wrapper.UserTriggerMinute}");

            return new OkResult();
        }

    }

}
