using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Swabbr.Core.Interfaces.BackgroundWorkers;
using Swabbr.Core.Interfaces.Services;
using System;
using System.Threading.Tasks;

namespace Swabbr.AzureFunctions.Functions
{

    /// <summary>
    /// Function for firing the <see cref="IVlogTriggerWorker"/>.
    /// </summary>
    public class VlogTriggerFunction
    {

        private readonly IVlogTriggerWorker _vlogTriggerWorker;
        private static bool hasSent = false;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public VlogTriggerFunction(IVlogTriggerWorker vlogTriggerWorker)
        {
            _vlogTriggerWorker = vlogTriggerWorker ?? throw new ArgumentNullException(nameof(vlogTriggerWorker));
        }

        /// <summary>
        /// Azure Timer Function to launch <see cref="IVlogTriggerWorker"/>.
        /// </summary>
        /// <param name="myTimer"><see cref="TimerInfo"/></param>
        /// <param name="log"><see cref="ILogger"/></param>
        [FunctionName("VlogTriggerFunction")]
        public async Task Run([TimerTrigger("*/10 * * * * *")]TimerInfo myTimer, ILogger log)
        {
            if (!hasSent)
            {
                hasSent = true;
            }
            else
            {
                return;
            }

            var id = Guid.NewGuid();
            log.LogInformation($"{nameof(VlogTriggerFunction)} fired at {DateTimeOffset.Now} - [{id}]");

            await _vlogTriggerWorker.SendAllRequestsAsync().ConfigureAwait(false);

            log.LogInformation($"{nameof(VlogTriggerFunction)} finished at {DateTimeOffset.Now} - [{id}]");
        }

    }

}
