using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Swabbr.Core.Interfaces.Services;
using System;
using System.Threading.Tasks;

namespace Swabbr.AzureFunctions.Functions
{

    /// <summary>
    /// Function for firing <see cref="IVlogTriggerService.ProcessVlogTriggersAsync(DateTimeOffset)"/>
    /// every minute.
    /// </summary>
    public sealed class VlogTriggerFunction
    {

        private readonly IVlogTriggerService _vlogTriggerService;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public VlogTriggerFunction(IVlogTriggerService vlogTriggerService)
        {
            _vlogTriggerService = vlogTriggerService ?? throw new ArgumentNullException(nameof(vlogTriggerService));
        }

        /// <summary>
        /// Azure Timer Function to launch <see cref="IVlogTriggerService"/>.
        /// </summary>
        /// <param name="myTimer"><see cref="TimerInfo"/></param>
        /// <param name="log"><see cref="ILogger"/></param>
        [FunctionName("VlogTriggerFunction")]
        public async Task Run([TimerTrigger("*/5 * * * *")]TimerInfo myTimer, ILogger log)
        {
            //var time = myTimer.ScheduleStatus.Next;

            //log.LogInformation($"{nameof(VlogTriggerFunction)} fired for {time.Year}/{time.Month}/{time.Day} {time.Hour}:{time.Minute} at {DateTimeOffset.Now}");

            //await _vlogTriggerService.ProcessVlogTriggersAsync(DateTimeOffset.Now).ConfigureAwait(false);

            //log.LogInformation($"{nameof(VlogTriggerFunction)} finished for {time.Year}/{time.Month}/{time.Day} {time.Hour}:{time.Minute} at {DateTimeOffset.Now}");
        }

    }

}
