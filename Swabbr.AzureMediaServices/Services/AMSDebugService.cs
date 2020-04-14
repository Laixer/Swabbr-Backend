using Microsoft.Azure.Management.Media;
using Microsoft.Azure.Management.Media.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Swabbr.AzureMediaServices.Configuration;
using Swabbr.AzureMediaServices.Extensions;
using Swabbr.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Swabbr.AzureMediaServices.Services
{

    /// <summary>
    /// Used for AMS debug.
    /// </summary>
    public sealed class AMSDebugService
    {

        private static string liveEventName => "debug-live-event";

        private readonly AMSConfiguration config;
        private readonly IStorageService _storageService;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public AMSDebugService(IOptions<AMSConfiguration> options,
            IStorageService storageService)
        {
            if (options == null) { throw new ArgumentNullException(nameof(options)); }
            options.Value.ThrowIfInvalid();
            config = options.Value;
            _storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
        }

        public async Task DoThingAsync()
        {
            try
            {
                var amsClient = await AMSClientFactory.GetClientAsync(config).ConfigureAwait(false);
                var liveEvent = await amsClient.LiveEvents.GetAsync(config.ResourceGroup, config.AccountName, liveEventName);
                liveEvent.Input.AccessToken = "my-access-token";

                var inputs = await amsClient.LiveEvents.ListAsync(config.ResourceGroup, config.AccountName).ConfigureAwait(false);

                await amsClient.LiveEvents.UpdateAsync(config.ResourceGroup, config.AccountName, liveEventName, liveEvent).ConfigureAwait(false);

                var updatedLiveEvent = await amsClient.LiveEvents.GetAsync(config.ResourceGroup, config.AccountName, liveEventName);
            }
            catch (Exception e)
            {
                throw;
            }
        }

    }

}
