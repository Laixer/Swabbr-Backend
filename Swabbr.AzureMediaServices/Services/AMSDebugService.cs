using Laixer.Utility.Extensions;
using Microsoft.Azure.Management.Media;
using Microsoft.Azure.Management.Media.Models;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Swabbr.AzureMediaServices.Clients;
using Swabbr.AzureMediaServices.Configuration;
using Swabbr.AzureMediaServices.Entities;
using Swabbr.AzureMediaServices.Extensions;
using Swabbr.AzureMediaServices.Utility;
using Swabbr.Core.Interfaces.Services;
using Swabbr.Core.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

                // Create a new job
                var reactionId = Guid.NewGuid();

                var jia = new JobInputAsset("asset name", start: new AbsoluteClipTime(TimeSpan.Zero), end: new AbsoluteClipTime(TimeSpan.FromSeconds(8)));

                var jobInput = new JobInputAsset(assetName: AMSNameGenerator.ReactionInputAssetName(reactionId));
                var jobOutputs = new JobOutput[] { new JobOutputAsset("outputassetname"), };
                var jobName = AMSNameGenerator.ReactionJobName(reactionId);
                var job = await amsClient.Jobs.CreateAsync(config.ResourceGroup, config.AccountName, "mytransform", jobName, new Job()
                {
                    Input = jobInput,
                    Outputs = jobOutputs
                }).ConfigureAwait(false);


                var response = await amsClient.Assets.ListContainerSasAsync(
                    config.ResourceGroup,
                    config.AccountName,
                    "assetinputname",
                    permissions: AssetContainerPermission.ReadWrite,
                    expiryTime: DateTime.UtcNow.AddHours(4).ToUniversalTime()).ConfigureAwait(false);

                var sasUri = new Uri(response.AssetContainerSasUrls.First()); // First means AMS access key 1 is used
                var container = new CloudBlobContainer(sasUri);
                var blob = container.GetBlockBlobReference(AMSNameGenerator.ReactionVideoFileName(reactionId));

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

        /// <summary>
        /// Extracts the video length in seconds.
        /// </summary>
        /// <param name="reactionId">Internal <see cref="Reaction"/> id</param>
        /// <param name="config"><see cref="AMSConfiguration"/></param>
        /// <returns>Video length in seconds</returns>
        public async Task<int> ExtractVideoLengthInSecondsAsync(Guid reactionId)
        {
            var client = await AMSClientFactory.GetClientAsync(config).ConfigureAwait(false);
            var outputAssetName = AMSNameGenerator.ReactionOutputAssetName(reactionId);
            var outputAsset = await client.Assets.GetAsync(config.ResourceGroup, config.AccountName, outputAssetName).ConfigureAwait(false);

            var sas = await _storageService.GetDownloadAccessUriForReactionContainerAsync(reactionId).ConfigureAwait(false);
            var container = new CloudBlobContainer(sas);

            CloudBlockBlob GetManifestBlob(IEnumerable<CloudBlockBlob> blobs)
            {
                foreach (var blob in blobs)
                {
                    if (Regex.IsMatch(blob.Name, AMSNameGenerator.OutputContainerMetadataFileNameRegex)) { return blob; }
                }
                throw new InvalidOperationException("Could not find manifest blob in container");
            }

            var manifestBlob = GetManifestBlob(await AMSRequestUtility.ListBlobsAsync(container).ConfigureAwait(false));

            async Task<JsonManifest> GetManifestAsync(CloudBlockBlob blob)
            {
                var stream = null as MemoryStream;
                try
                {
                    stream = new MemoryStream();
                    await blob.DownloadToStreamAsync(stream);
                    var serializer = new JsonSerializer();
                    stream.Position = 0;
                    using (var streamReader = new StreamReader(stream))
                    {
                        using (var jsonTextReader = new JsonTextReader(streamReader))
                        {
                            var result = serializer.Deserialize<JsonManifest>(jsonTextReader);
                            if (result == null) { throw new FormatException("Could not deserialize AMS metadata"); }
                            if (!result.AssetFile.Any()) { throw new FormatException("AMS metadata contained no data"); }
                            if (!result.AssetFile.First().VideoTracks.Any()) { throw new FormatException("AMS metadata contained no video tracks"); }
                            return result;
                        }
                    }
                }
                finally
                {
                    stream?.Close();
                }
            }

            var manifest = await GetManifestAsync(manifestBlob).ConfigureAwait(false);
            var frames = manifest.AssetFile.First().VideoTracks.First().NumberOfFrames;
            var frameRate = manifest.AssetFile.First().VideoTracks.First().FrameRate;
            return (int)Math.Ceiling(frames / frameRate);
        }

        /// <summary>
        /// Creates a new asset for a reaction upload and returns the stream to which we can upload.
        /// </summary>
        /// <param name="reactionId"></param>
        /// <returns><see cref="StreamWithEntityIdWrapper"/></returns>
        [Obsolete("No longer used, we use SAS tokens now")]
        public async Task<StreamWithEntityIdWrapper> GetStreamForReactionUploadAsync(Guid reactionId)
        {
            reactionId.ThrowIfNullOrEmpty();

            // Create input asset
            var amsClient = await AMSClientFactory.GetClientAsync(config).ConfigureAwait(false);
            var assetInputName = AMSNameGenerator.ReactionInputAssetName(reactionId);
            var assetParams = new Asset(container: assetInputName);
            var assetInput = await amsClient.Assets.CreateOrUpdateAsync(config.ResourceGroup, config.AccountName, assetInputName, assetParams).ConfigureAwait(false);

            // Get the blob
            var response = await amsClient.Assets.ListContainerSasAsync(
                config.ResourceGroup,
                config.AccountName,
                assetInputName,
                permissions: AssetContainerPermission.ReadWrite,
                expiryTime: DateTime.UtcNow.AddHours(4).ToUniversalTime()).ConfigureAwait(false);
            var sasUri = new Uri(response.AssetContainerSasUrls.First()); // First means AMS access key 1 is used
            var container = new CloudBlobContainer(sasUri);
            var blob = container.GetBlockBlobReference(AMSNameGenerator.ReactionVideoFileName(reactionId));

            // TODO Is this safe? It's captured in a using block, but this is bug sensitive!
            return new StreamWithEntityIdWrapper(await blob.OpenWriteAsync().ConfigureAwait(false), reactionId);
        }

    }

}
