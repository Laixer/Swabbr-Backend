using Laixer.Utility.Extensions;
using Microsoft.Azure.Management.Media;
using Microsoft.Azure.Management.Media.Models;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
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
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Swabbr.AzureMediaServices.Services
{

    /// <summary>
    /// Transcoding wrapper based on Azure Media Services.
    /// This has no knowlede of our data store.
    /// </summary>
    public sealed class AMSTranscodingService : ITranscodingService
    {

        private readonly AMSConfiguration config;
        private readonly IStorageService _storageService;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public AMSTranscodingService(IOptions<AMSConfiguration> options,
            IStorageService storageService)
        {
            if (options == null) { throw new ArgumentNullException(nameof(options)); }
            options.Value.ThrowIfInvalid();
            config = options.Value;
            _storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
        }

        /// <summary>
        /// Creates a new asset for a reaction upload and returns the stream to which we can upload.
        /// </summary>
        /// <param name="reactionId"></param>
        /// <returns><see cref="StreamWithEntityIdWrapper"/></returns>
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

        /// <summary>
        /// Should be called when a reaction is uploaded as an input asset.
        /// </summary>
        /// <param name="reactionId">Internal <see cref="Reaction"/> id</param>
        /// <returns><see cref="Task"/></returns>
        public async Task ProcessReactionAsync(Guid reactionId)
        {
            reactionId.ThrowIfNullOrEmpty();

            // Create output asset
            var amsClient = await AMSClientFactory.GetClientAsync(config).ConfigureAwait(false);
            var assetOutputName = AMSNameGenerator.ReactionOutputAssetName(reactionId);
            var assetOutput = await amsClient.Assets.CreateOrUpdateAsync(config.ResourceGroup, config.AccountName, assetOutputName, new Asset(container: assetOutputName)).ConfigureAwait(false);

            // TODO Maybe we can name the metadata file? Would be great!
            // TODO Apparently the asset can just return the metadata
            // https://stackoverflow.com/questions/29296205/how-to-get-the-duration-of-a-video-from-the-azure-media-services

            // Get the transform
            var transformName = AMSNameConstants.ReactionTransformName;
            await EnsureTransformAsync(amsClient, transformName).ConfigureAwait(false);

            // Create a new job
            var jobInput = new JobInputAsset(assetName: AMSNameGenerator.ReactionInputAssetName(reactionId));
            var jobOutputs = new JobOutput[] { new JobOutputAsset(assetOutputName), };
            var jobName = AMSNameGenerator.ReactionJobName(reactionId);
            var job = await amsClient.Jobs.CreateAsync(config.ResourceGroup, config.AccountName, transformName, jobName, new Job()
            {
                Input = jobInput,
                Outputs = jobOutputs
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reactionId"></param>
        /// <returns></returns>
        public async Task CleanupReactionAsync(Guid reactionId)
        {
            reactionId.ThrowIfNullOrEmpty();

            await _storageService.CleanupReactionStorageAsync(reactionId).ConfigureAwait(false);

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
        /// Creates and updates a transform set to our needs.
        /// </summary>
        /// <remarks>
        /// TODO Check this
        /// </remarks>
        /// <param name="amsClient"></param>
        /// <param name="transformName"></param>
        /// <returns></returns>
        private async Task EnsureTransformAsync(IAzureMediaServicesClient amsClient, string transformName)
        {
            var transform = await amsClient.Transforms.GetAsync(config.ResourceGroup, config.AccountName, transformName).ConfigureAwait(false);

            var outputs = new TransformOutput[]
            {
                new TransformOutput(
                    new StandardEncoderPreset(
                        codecs: new Codec[]
                        {
                            // Audio codec for video file
                            new AacAudio(
                                channels: 2,
                                samplingRate: 48000,
                                bitrate: 128000,
                                profile: AacAudioProfile.AacLc
                            ),

                            // Video file
                            new H264Video (
                                keyFrameInterval:TimeSpan.FromSeconds(2),
                                layers:  new H264Layer[]
                                {
                                    new H264Layer (
                                        bitrate: 1000000,
                                        width: "1280",
                                        height: "720",
                                        label: "HD"
                                    )
                                }
                            ),

                            // Thumbnails
                            new PngImage(
                                start: "25%",
                                layers: new PngLayer[]{
                                    new PngLayer(
                                        width: "50%",
                                        height: "50%"
                                    )
                                }
                            )
                        },

                        // Name formatting
                        formats: new Format[]
                        {
                            new Mp4Format(
                                filenamePattern:"video-{Basename}{Extension}"
                            ),
                            new PngFormat(
                                filenamePattern:"thumbnail-{Basename}-{Index}{Extension}"
                            )
                        }
                    ),
                    onError: OnErrorType.StopProcessingJob,
                    relativePriority: Priority.Normal
                )
            };

            await amsClient.Transforms.CreateOrUpdateAsync(config.ResourceGroup, config.AccountName, transformName, outputs, AMSNameConstants.ReactionTransformDescription);
        }

    }

}
