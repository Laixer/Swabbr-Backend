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
using System.Linq;
using System.Threading.Tasks;

namespace Swabbr.AzureFunctions.Functions
{

    /// <summary>
    /// Gets a collection of users for a given trigger minute.
    /// </summary>
    public sealed class LogicAppGetUsersFunction
    {

        private readonly IUserService _userService;
        private readonly IHashDistributionService _hashDistributionService;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public LogicAppGetUsersFunction(IUserService userService,
            IHashDistributionService hashDistributionService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _hashDistributionService = hashDistributionService ?? throw new ArgumentNullException(nameof(hashDistributionService));
        }

        /// <summary>
        /// Gets all <see cref="Core.Entities.SwabbrUser"/>s for a given trigger
        /// minute.
        /// </summary>
        /// <param name="req"><see cref="HttpRequest"/></param>
        /// <param name="log"><see cref="ILogger"/></param>
        /// <returns><see cref="Core.Entities.SwabbrUser"/> id collection</returns>
        [FunctionName(nameof(LogicAppGetUsersFunction))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            if (req == null) { throw new ArgumentNullException(nameof(req)); }

            using var streamReader = new StreamReader(req.Body);
            var wrapper = JsonConvert.DeserializeObject<TriggerMinuteWrapper>(await streamReader.ReadToEndAsync().ConfigureAwait(false));
            if (wrapper.TriggerMinute.IsNullOrEmpty()) { throw new ArgumentNullException("Missing trigger minute"); }

            log.LogInformation($"{nameof(LogicAppGetUsersFunction)} - Getting users for trigger minute {wrapper.TriggerMinute}");

            var users = await _userService.GetAllVloggableUserMinifiedAsync().ConfigureAwait(false);
            var selectedUsers = _hashDistributionService.GetForMinute(users, wrapper.TriggerMinute);

            log.LogInformation($"{nameof(LogicAppGetUsersFunction)} - Returning {selectedUsers.Count()} users for trigger minute {wrapper.TriggerMinute}");
            return new OkObjectResult(selectedUsers.Select(x => x.Id));
        }

    }

}
