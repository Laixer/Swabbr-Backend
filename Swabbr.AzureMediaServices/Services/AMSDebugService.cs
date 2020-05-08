using Laixer.Utility.Extensions;
using Microsoft.Azure.Management.Media;
using Microsoft.Azure.Management.Media.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Swabbr.AzureMediaServices.Clients;
using Swabbr.AzureMediaServices.Configuration;
using Swabbr.AzureMediaServices.Extensions;
using Swabbr.AzureMediaServices.Utility;
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
                var liveEvent = await amsClient.LiveEvents.GetAsync(config.ResourceGroup, config.AccountName, "live-event-RHSlLRPVbklIbd7").ConfigureAwait(false);
                var liveEvent2 = await amsClient.LiveEvents.GetAsync(config.ResourceGroup, config.AccountName, "614b691783964cee8e3c45028f9e749a").ConfigureAwait(false);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task DeleteAllAssetsAsync()
        {
            var amsClient = await AMSClientFactory.GetClientAsync(config).ConfigureAwait(false);
            var assets = await amsClient.Assets.ListAsync(config.ResourceGroup, config.AccountName).ConfigureAwait(false);
            foreach (var asset in assets)
            {
                await amsClient.Assets.DeleteAsync(config.ResourceGroup, config.AccountName, asset.Name).ConfigureAwait(false);
            }
        }

        public async Task<LiveEvent> GetLiveEventAsync(string liveEventName)
        {
            liveEventName.ThrowIfNullOrEmpty();
            var amsClient = await AMSClientFactory.GetClientAsync(config).ConfigureAwait(false);
            return await amsClient.LiveEvents.GetAsync(config.ResourceGroup, config.AccountName, liveEventName).ConfigureAwait(false);
        }        
        
        public async Task<LiveOutput> GetLiveOutputAsync(string liveEventName, Guid correspondingVlogId)
        {
            liveEventName.ThrowIfNullOrEmpty();
            correspondingVlogId.ThrowIfNullOrEmpty();
            var amsClient = await AMSClientFactory.GetClientAsync(config).ConfigureAwait(false);
            var liveOutputName = AMSNameGenerator.VlogLiveOutputName(correspondingVlogId);
            return await amsClient.LiveOutputs.GetAsync(config.ResourceGroup, config.AccountName, liveEventName, liveOutputName).ConfigureAwait(false);
        }

        public async Task DeleteLiveEventAsync(string liveEventName)
        {
            var amsClient = await AMSClientFactory.GetClientAsync(config).ConfigureAwait(false);
            await amsClient.LiveEvents.DeleteAsync(config.ResourceGroup, config.AccountName, liveEventName).ConfigureAwait(false);
        }

        public async Task DeleteLiveOutputAsync(string liveEventName, Guid correspondingVlogId)
        {
            liveEventName.ThrowIfNullOrEmpty();
            correspondingVlogId.ThrowIfNullOrEmpty();
            var amsClient = await AMSClientFactory.GetClientAsync(config).ConfigureAwait(false);
            await amsClient.LiveOutputs.DeleteAsync(config.ResourceGroup, config.AccountName, liveEventName, AMSNameGenerator.VlogLiveOutputName(correspondingVlogId)).ConfigureAwait(false);
        }

    }

}
