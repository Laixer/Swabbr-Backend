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
using System.Collections.Generic;
using System.IO;
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
            var wrapper = JsonConvert.DeserializeObject<TriggerMinuteWrapper>(await new StreamReader(req.Body).ReadToEndAsync());
            if (wrapper.TriggerMinute.IsNullOrEmpty()) { throw new ArgumentNullException("Missing trigger minute"); }

            if (wrapper.TriggerMinute.GetMinutes() % 10 == 0)
            {
                return new OkObjectResult(new List<Guid> { new Guid("e2c8b3f3-6882-4d12-bfcf-ac46b1b3d2ee") });
            }
            else
            {
                return new OkObjectResult(new List<Guid>());
            }
        }

    }

}
