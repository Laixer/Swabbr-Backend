using Laixer.Utility.Extensions;
using Microsoft.Azure.Management.Media;
using Microsoft.Azure.Management.Media.Models;
using Microsoft.Extensions.Options;
using Swabbr.AzureMediaServices.Configuration;
using Swabbr.AzureMediaServices.Extensions;
using Swabbr.AzureMediaServices.Interfaces.Clients;
using Swabbr.AzureMediaServices.Utility;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Types;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Swabbr.AzureMediaServices.Clients
{

    /// <summary>
    /// Communicates with Azure Media Services.
    /// </summary>
    public sealed class AMSClient : IAMSClient
    {

        private readonly AMSConfiguration _config;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public AMSClient(IOptions<AMSConfiguration> config)
        {
            _config = config.Value ?? throw new ArgumentNullException(nameof(config.Value));
            _config.ThrowIfInvalid();
        }

        /// <summary>
        /// Creates a new <see cref="LiveEvent"/> in AMS.
        /// </summary>
        /// <remarks>
        /// TODO Maybe centralize these livestream parameters
        /// </remarks>
        /// <param name="livestreamId">Internal <see cref="Core.Entities.Livestream"/> id</param>
        /// <returns><see cref="LiveEvent"/></returns>
        public async Task<LiveEvent> CreateLiveEventAsync(Guid livestreamId)
        {
            livestreamId.ThrowIfNullOrEmpty();

            // First do the checks
            await EnsureLivestreamTransformExistsAsync().ConfigureAwait(false);
            await EnsureStreamingEndpointRunningAsync().ConfigureAwait(false);

            // Then create
            var amsClient = await AMSClientFactory.GetClientAsync(_config).ConfigureAwait(false);
            var liveEventName = AMSNameGenerator.LiveEventName();
            var liveEventRequest = new LiveEvent
            {
                Location = _config.Location,
                Input = new LiveEventInput
                {
                    StreamingProtocol = "RTMP",
                    AccessControl = new LiveEventInputAccessControl
                    {
                        Ip = new IPAccessControl
                        {
                            Allow = new List<IPRange> { new IPRange("Allow All", "0.0.0.0", 0) }
                        }
                    }
                },
                VanityUrl = false,
            };
            return await amsClient.LiveEvents.CreateAsync(_config.ResourceGroup, _config.AccountName, liveEventName, liveEventRequest);
        }

        /// <summary>
        /// Creates a new <see cref="LiveOutput"/> in AMS.
        /// </summary>
        /// <remarks>
        /// This creates the <see cref="Asset"/>, the <see cref="LiveOutput"/>
        /// and a new <see cref="StreamingLocator"/>.
        /// </remarks>
        /// <param name="livestreamId">Internal <see cref="Core.Entities.Livestream"/> id</param>
        /// <param name="liveEventName">AMS <see cref="LiveEvent"/> name</param>
        /// <param name="correspondingVlogId">Internal <see cref="Core.Entities.Vlog"/> id
        /// that corresponds to the livestream</param>
        /// <returns><see cref="LiveOutput"/></returns>
        public async Task<LiveOutput> CreateLiveOutputAsync(Guid correspondingVlogId, string liveEventName)
        {
            liveEventName.ThrowIfNullOrEmpty();
            correspondingVlogId.ThrowIfNullOrEmpty();

            var amsClient = await AMSClientFactory.GetClientAsync(_config).ConfigureAwait(false);

            // First do the checks
            var contentKeyPolicy = await EnsureContentKeyPolicyExistsAsync().ConfigureAwait(false);

            // Create asset
            var liveOutputAssetName = AMSNameGenerator.VlogLiveOutputAssetName(correspondingVlogId);
            var liveOutputAsset = await CreateAssetAsync(liveOutputAssetName).ConfigureAwait(false);

            // Create output
            var liveOutputName = AMSNameGenerator.VlogLiveOutputName(correspondingVlogId);
            var liveOutputRequest = new LiveOutput
            {
                AssetName = liveOutputAssetName
            };
            return await amsClient.LiveOutputs.CreateAsync(_config.ResourceGroup, _config.AccountName, liveEventName, liveOutputName, liveOutputRequest).ConfigureAwait(false);
        }

        /// <summary>
        /// Creates a new <see cref="StreamingLocator"/> in AMS.
        /// </summary>
        /// <param name="correspondingVlogId">Internal <see cref="Vlog"/> id</param>
        /// <param name="liveEventName">AMS <see cref="LiveEvent"/> name</param>
        /// <returns><see cref="StreamingLocator"/></returns>
        public async Task<StreamingLocator> CreateLivestreamVlogStreamingLocatorAsync(Guid correspondingVlogId, string liveEventName)
        {
            correspondingVlogId.ThrowIfNullOrEmpty();
            liveEventName.ThrowIfNullOrEmpty();

            var amsClient = await AMSClientFactory.GetClientAsync(_config).ConfigureAwait(false);

            var liveOutputAssetName = AMSNameGenerator.VlogLiveOutputAssetName(correspondingVlogId);
            var streamingLocatorName = AMSNameGenerator.VlogStreamingLocatorName(correspondingVlogId);
            var streamingLocatorRequest = new StreamingLocator
            {
                AssetName = liveOutputAssetName,
                StreamingPolicyName = PredefinedStreamingPolicy.ClearKey,
                DefaultContentKeyPolicyName = AMSNameConstants.ContentKeyPolicyName
            };
            return await amsClient.StreamingLocators.CreateAsync(_config.ResourceGroup, _config.AccountName, streamingLocatorName, streamingLocatorRequest).ConfigureAwait(false);
        }

        /// <summary>
        /// Makes sure that the <see cref="Core.Entities.Livestream"/> 
        /// <see cref="Transform"/>exists on the Azure Media Services side.
        /// </summary>
        /// <remarks>
        /// TODO This can be optimized, shouldn't need to run every single time.
        /// </remarks>
        /// <returns><see cref="Task"/></returns>
        public async Task EnsureLivestreamTransformExistsAsync()
        {
            var amsClient = await AMSClientFactory.GetClientAsync(_config).ConfigureAwait(false);
            var livestreamTransformName = AMSNameConstants.LivestreamTransformName;

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

            await amsClient.Transforms.CreateOrUpdateAsync(_config.ResourceGroup, _config.AccountName, livestreamTransformName, outputs, AMSNameConstants.LivestreamTransformDescription);
        }

        public Task EnsureReactionTransformExistsAsync()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Makes sure that the used <see cref="StreamingEndpoint"/> is in the
        /// running state.
        /// </summary>
        /// <returns><see cref="Task"/></returns>
        public async Task EnsureStreamingEndpointRunningAsync()
        {
            var amsClient = await AMSClientFactory.GetClientAsync(_config).ConfigureAwait(false);
            var streamingEndpointName = AMSNameConstants.StreamingEndpointName;
            var endpoint = await amsClient.StreamingEndpoints.GetAsync(_config.ResourceGroup, _config.AccountName, streamingEndpointName).ConfigureAwait(false);

            if (endpoint == null)
            {
                var streamingEndpointRequest = new StreamingEndpoint
                {
                    Location = _config.Location
                };
                await amsClient.StreamingEndpoints.CreateAsync(_config.ResourceGroup, _config.AccountName, streamingEndpointName, streamingEndpointRequest).ConfigureAwait(false);
            }

            await amsClient.StreamingEndpoints.StartAsync(_config.ResourceGroup, _config.AccountName, streamingEndpointName).ConfigureAwait(false);
        }

        /// <summary>
        /// Ensures our required <see cref="StreamingPolicy"/> exists.
        /// </summary>
        /// <remarks>
        /// First creates the content key policy, then the streaming policy.
        /// TODO Question --> Is it smart to do this in code, since any key will work? Maybe this should be setup in our portal for security?
        /// </remarks>
        /// <returns><see cref="Task"/></returns>
        public async Task<ContentKeyPolicy> EnsureContentKeyPolicyExistsAsync()
        {
            var amsClient = await AMSClientFactory.GetClientAsync(_config).ConfigureAwait(false);
            var policy = await amsClient.ContentKeyPolicies.GetAsync(_config.ResourceGroup, _config.AccountName, AMSNameConstants.ContentKeyPolicyName).ConfigureAwait(false);

            if (policy == null)
            {
                var primaryKey = new ContentKeyPolicySymmetricTokenKey(new UTF8Encoding().GetBytes(_config.TokenSecret));
                var requiredClaims = new List<ContentKeyPolicyTokenClaim>()
                {
                    ContentKeyPolicyTokenClaim.ContentKeyIdentifierClaim
                };

                var options = new List<ContentKeyPolicyOption>()
                {
                    new ContentKeyPolicyOption()
                    {
                        Name = AMSNameConstants.ContentKeyPolicyOptionName,
                        Configuration = new ContentKeyPolicyClearKeyConfiguration(),
                        Restriction = new ContentKeyPolicyTokenRestriction()
                        {
                            Audience = _config.TokenAudience,
                            Issuer = _config.TokenIssuer,
                            PrimaryVerificationKey = primaryKey,
                            RequiredClaims = requiredClaims,
                            RestrictionTokenType = ContentKeyPolicyRestrictionTokenType.Jwt
                        }
                    }
                };

                policy = await amsClient.ContentKeyPolicies.CreateOrUpdateAsync(_config.ResourceGroup, _config.AccountName, AMSNameConstants.ContentKeyPolicyName, options).ConfigureAwait(false);
            }

            return policy;
        }

        /// <summary>
        /// Generates a new <see cref="LivestreamUpstreamDetails"/> object for
        /// a given <see cref="LiveEvent"/>.
        /// </summary>
        /// <param name="livestreamId">Internal <see cref="Core.Entities.Livestream"/> id</param>
        /// <param name="vlogId">Internal <see cref="Core.Entities.SwabbrUser"/> id</param>
        /// <returns><see cref="LivestreamUpstreamDetails"/></returns>
        public async Task<LivestreamUpstreamDetails> GetUpstreamDetailsAsync(Guid livestreamId, Guid vlogId, string liveEventName)
        {
            livestreamId.ThrowIfNullOrEmpty();
            vlogId.ThrowIfNullOrEmpty();
            liveEventName.ThrowIfNullOrEmpty();

            var amsClient = await AMSClientFactory.GetClientAsync(_config).ConfigureAwait(false);
            var liveEvent = await amsClient.LiveEvents.GetAsync(_config.ResourceGroup, _config.AccountName, liveEventName).ConfigureAwait(false);

            // External checks
            if (liveEvent.ResourceState != LiveEventResourceState.Running) { throw new ExternalErrorException("Live Event not in running state"); }
            // TODO Check if nobody is already streaming

            return new LivestreamUpstreamDetails
            {
                ApplicationName = null,
                HostPort = 2935, // TODO Extract, don't hardcode
                HostServer = GetUriFromLiveEvent(liveEvent),
                LivestreamId = livestreamId,
                Password = null,
                StreamKey = liveEvent.Input.AccessToken,
                Username = null,
                VlogId = vlogId
            };
        }

        /// <summary>
        /// Gets the <see cref="StreamingLocator"/> identifier key for a <see cref="Vlog"/>.
        /// </summary>
        /// <param name="vlogId">Internal <see cref="Vlog"/> id</param>
        /// <returns>Key</returns>
        public async Task<string> GetVlogStreamingLocatorKeyIdentifierAsync(Guid vlogId)
        {
            vlogId.ThrowIfNullOrEmpty();

            var amsClient = await AMSClientFactory.GetClientAsync(_config).ConfigureAwait(false);
            var streamingLocatorName = AMSNameGenerator.VlogStreamingLocatorName(vlogId);
            var streamingLocator = await amsClient.StreamingLocators.GetAsync(_config.ResourceGroup, _config.AccountName, streamingLocatorName).ConfigureAwait(false);
            return streamingLocator.ContentKeys.First().Id.ToString();
        }

        /// <summary>
        /// Gets a <see cref="StreamingLocator"/> for a vlog.
        /// </summary>
        /// <param name="vlogId">Internal <see cref="Vlog"/> id (can belong
        /// to a <see cref="Livestream"/>)</param>
        /// <returns><see cref="StreamingLocator"/></returns>
        public async Task<IEnumerable<string>> GetVlogStreamingLocatorPathsAsync(Guid vlogId)
        {
            vlogId.ThrowIfNullOrEmpty();

            var amsClient = await AMSClientFactory.GetClientAsync(_config).ConfigureAwait(false);
            var streamingLocatorName = AMSNameGenerator.VlogStreamingLocatorName(vlogId);
            var paths = await amsClient.StreamingLocators.ListPathsAsync(_config.ResourceGroup, _config.AccountName, streamingLocatorName).ConfigureAwait(false);

            // TODO This just returns EVERYTHING at the moment, doesn't consider protocols or anything
            var result = new Collection<string>();
            foreach (var streamingPath in paths.StreamingPaths)
            {
                foreach (var path in streamingPath.Paths)
                {
                    result.Add(path);
                }
            }
            return result;
        }

        public Task<StreamingLocator> GetStreamingLocatorForReactionAsync(Guid reactionId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the <see cref="StreamingEndpoint"/> host name.
        /// </summary>
        /// <returns>Host name</returns>
        public async Task<string> GetStreamingEndpointHostNameAsync()
        {
            var amsClient = await AMSClientFactory.GetClientAsync(_config).ConfigureAwait(false);
            var streamingEndpoint = await amsClient.StreamingEndpoints.GetAsync(_config.ResourceGroup, _config.AccountName, AMSNameConstants.StreamingEndpointName).ConfigureAwait(false);
            return $"https://{streamingEndpoint.HostName}";
        }

        /// <summary>
        /// Starts a <see cref="LiveEvent"/> in AMS.
        /// </summary>
        /// <param name="liveEventName">AMS <see cref="LiveEvent"/> name</param>
        /// <returns><see cref="Task"/></returns>
        public async Task StartLiveEventAsync(string liveEventName)
        {
            liveEventName.ThrowIfNullOrEmpty();
            var amsClient = await AMSClientFactory.GetClientAsync(_config).ConfigureAwait(false);
            await amsClient.LiveEvents.StartAsync(_config.ResourceGroup, _config.AccountName, liveEventName).ConfigureAwait(false);
        }

        /// <summary>
        /// Stops a <see cref="LiveEvent"/> in AMS.
        /// </summary>
        /// <param name="liveEventName">AMS <see cref="LiveEvent"/> name</param>
        /// <returns><see cref="Task"/></returns>
        public async Task StopLiveEventAsync(string liveEventName)
        {
            liveEventName.ThrowIfNullOrEmpty();
            var amsClient = await AMSClientFactory.GetClientAsync(_config).ConfigureAwait(false);
            await amsClient.LiveEvents.StopAsync(_config.ResourceGroup, _config.AccountName, liveEventName).ConfigureAwait(false);
        }

        /// <summary>
        /// Stops a <see cref="LiveOutput"/> in AMS.
        /// </summary>
        /// <param name="vlogId">Internal <see cref="Vlog"/> id</param>
        /// <param name="liveEventName">AMS <see cref="LiveEvent"/> id</param>
        /// <returns><see cref="Task"/></returns>
        public async Task StopLiveOutputAsync(Guid vlogId, string liveEventName)
        {
            vlogId.ThrowIfNullOrEmpty();
            liveEventName.ThrowIfNullOrEmpty();

            var amsClient = await AMSClientFactory.GetClientAsync(_config).ConfigureAwait(false);
            await amsClient.LiveOutputs.DeleteAsync(_config.ResourceGroup, _config.AccountName, liveEventName, AMSNameGenerator.VlogLiveOutputName(vlogId)).ConfigureAwait(false);
        }

        /// <summary>
        /// Creates a new <see cref="Asset"/> in AMS.
        /// </summary>
        /// <param name="assetName">AMS <see cref="Asset"/> name</param>
        /// <returns><see cref="Asset"/></returns>
        private async Task<Asset> CreateAssetAsync(string assetName)
        {
            assetName.ThrowIfNullOrEmpty();

            var amsClient = await AMSClientFactory.GetClientAsync(_config).ConfigureAwait(false);
            var assetRequest = new Asset();
            return await amsClient.Assets.CreateOrUpdateAsync(_config.ResourceGroup, _config.AccountName, assetName, assetRequest).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets a correct RTMPS url from the <see cref="LiveEvent"/>.
        /// </summary>
        /// <param name="liveEvent"></param>
        /// <param name="protocol"></param>
        /// <returns></returns>
        private Uri GetUriFromLiveEvent(LiveEvent liveEvent, string protocol = "RTMP")
        {
            var endpoints = liveEvent.Input.Endpoints;
            if (!endpoints.Any()) { throw new ExternalErrorException("Live Event has no endpoints"); }
            foreach (var endpoint in endpoints)
            {
                if (endpoint.Protocol.Equals(protocol) &&
                    endpoint.Url.Substring(0, 5).Equals("rtmps"))
                {
                    return new Uri(endpoint.Url);
                }
            }
            throw new ExternalErrorException($"Could not find Live Event endpoint for protocol {protocol}");
        }

    }

}
