﻿using Swabbr.Core.Extensions;
using Microsoft.Azure.Management.Media;
using Microsoft.Azure.Management.Media.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest.Azure.Authentication;
using Swabbr.AzureMediaServices.Configuration;
using Swabbr.AzureMediaServices.Exceptions;
using Swabbr.AzureMediaServices.Extensions;
using Swabbr.AzureMediaServices.Utility;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Types;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swabbr.AzureMediaServices.Clients
{
    // TODO This is coupled with 111 different types from 24 different namespaces. Clean up?
    /// <summary>
    ///     Communicates with Azure Media Services.
    /// </summary>
    public class AMSClient : IDisposable
    {
        private readonly AMSConfiguration _config;
        private readonly ILogger<AMSClient> _logger;
        private readonly IAzureMediaServicesClient amsClient;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public AMSClient(IOptions<AMSConfiguration> config,
            ILogger<AMSClient> logger)
        {
            if (config == null) { throw new ArgumentNullException(nameof(config)); }
            _config = config.Value ?? throw new ArgumentNullException(nameof(config.Value));
            _config.ThrowIfInvalid();
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            // Create client only once.
            // TODO Make lazy, future optimalization.
            amsClient = Task.Run(() => BuildClientAsync()).Result;
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

            try
            {
                // First do the checks
                await EnsureLivestreamTransformExistsAsync().ConfigureAwait(false);
                await EnsureStreamingEndpointRunningAsync().ConfigureAwait(false);

                // Then create
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
                    // VanityUrl = false, // What does this do?
                    Encoding = new LiveEventEncoding
                    {
                        EncodingType = LiveEventEncodingType.None
                    }
                };
                return await amsClient.LiveEvents.CreateAsync(_config.ResourceGroup, _config.AccountName, AMSNameGenerator.LiveEventName, liveEventRequest).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw new ExternalAMSErrorException("Could not create new live event", e);
            }
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

            try
            {
                // First do the checks
                await EnsureContentKeyPolicyExistsAsync().ConfigureAwait(false);

                // Create asset
                var liveOutputAssetName = AMSNameGenerator.VlogLiveOutputAssetName(correspondingVlogId);
                await CreateAssetAsync(liveOutputAssetName).ConfigureAwait(false);

                // Create output
                var liveOutputRequest = new LiveOutput
                {
                    AssetName = liveOutputAssetName,
                    ArchiveWindowLength = TimeSpan.FromHours(AMSConstants.LiveOutputArchiveWindowLengthHours)
                };
                return await amsClient.LiveOutputs.CreateAsync(_config.ResourceGroup, _config.AccountName, liveEventName,
                    AMSNameGenerator.VlogLiveOutputName(correspondingVlogId), liveOutputRequest).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw new ExternalAMSErrorException("Could not create live output", e);
            }
        }

        /// <summary>
        /// Creates a new <see cref="StreamingLocator"/> for a <see cref="Core.Entities.Reaction"/>.
        /// </summary>
        /// <param name="reactionId">Internal <see cref="Core.Entities.Reaction"/> id</param>
        /// <returns><see cref="Task"/></returns>
        public async Task CreateReactionStreamingLocatorAsync(Guid reactionId)
        {
            reactionId.ThrowIfNullOrEmpty();

            try
            {
                await amsClient.StreamingLocators.CreateAsync(_config.ResourceGroup, _config.AccountName, AMSNameGenerator.ReactionStreamingLocatorName(reactionId), new StreamingLocator
                {
                    AssetName = AMSNameGenerator.ReactionOutputAssetName(reactionId),
                    StreamingPolicyName = PredefinedStreamingPolicy.ClearKey,
                    DefaultContentKeyPolicyName = AMSConstants.ContentKeyPolicyName
                }).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw new ExternalAMSErrorException("Could not create reaction streaming locator", e);
            }
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

            try
            {
                var liveOutputAssetName = AMSNameGenerator.VlogLiveOutputAssetName(correspondingVlogId);
                var streamingLocatorName = AMSNameGenerator.VlogStreamingLocatorName(correspondingVlogId);
                var streamingLocatorRequest = new StreamingLocator
                {
                    AssetName = liveOutputAssetName,
                    StreamingPolicyName = PredefinedStreamingPolicy.ClearKey,
                    DefaultContentKeyPolicyName = AMSConstants.ContentKeyPolicyName
                };
                return await amsClient.StreamingLocators.CreateAsync(_config.ResourceGroup, _config.AccountName, streamingLocatorName, streamingLocatorRequest).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw new ExternalAMSErrorException("Could not create vlog streaming locator", e);
            }
        }

        /// <summary>
        /// Makes sure that the <see cref="Core.Entities.Livestream"/> 
        /// <see cref="Transform"/>exists on the Azure Media Services side.
        /// </summary>
        /// <returns><see cref="Task"/></returns>
        public async Task EnsureLivestreamTransformExistsAsync()
        {
            try
            {
                var outputs = new TransformOutput[]
                {
                    new TransformOutput(
                        new StandardEncoderPreset(
                            codecs: new Codec[]
                            {
                                AMSConstants.LivestreamAudioCodec(),
                                AMSConstants.LivestreamVideoCodec(),
                                AMSConstants.LivestreamThumbnailCodec()
                            },
                            // Name formatting
                            formats: new Format[]
                            {
                                new Mp4Format(AMSConstants.FormatVideoFileNamePattern),
                                new PngFormat(AMSConstants.FormatThumbnailFileNamePattern)
                            }
                        ),
                        onError: OnErrorType.StopProcessingJob,
                        relativePriority: Priority.Normal
                    )
                };

                await amsClient.Transforms.CreateOrUpdateAsync(_config.ResourceGroup,
                    _config.AccountName,
                    AMSConstants.LivestreamTransformName,
                    outputs,
                    AMSConstants.LivestreamTransformDescription).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw new ExternalAMSErrorException("Could not create or update livestream transform", e);
            }
        }

        /// <summary>
        /// Make sure that the <see cref="Core.Entities.Reaction"/> <see cref="Transform"/> exists.
        /// </summary>
        /// <returns><see cref="Task"/></returns>
        public async Task EnsureReactionTransformExistsAsync()
        {
            try
            {
                var outputs = new TransformOutput[]
                {
                    new TransformOutput(
                        new StandardEncoderPreset(
                            codecs: new Codec[]
                            {
                                AMSConstants.LivestreamAudioCodec(),
                                AMSConstants.LivestreamVideoCodec(),
                                AMSConstants.LivestreamThumbnailCodec()
                            },
                            // Name formatting
                            formats: new Format[]
                            {
                                new Mp4Format(AMSConstants.FormatVideoFileNamePattern),
                                new PngFormat(AMSConstants.FormatThumbnailFileNamePattern)
                            }
                        ),
                        onError: OnErrorType.StopProcessingJob,
                        relativePriority: Priority.Normal
                    )
                };

                await amsClient.Transforms.CreateOrUpdateAsync(_config.ResourceGroup,
                    _config.AccountName,
                    AMSConstants.ReactionTransformName,
                    outputs,
                    AMSConstants.ReactionTransformDescription).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw new ExternalAMSErrorException("Could not create or update reaction transform", e);
            }
        }

        /// <summary>
        /// Makes sure that the used <see cref="StreamingEndpoint"/> is in the
        /// running state.
        /// </summary>
        /// <returns><see cref="Task"/></returns>
        public async Task EnsureStreamingEndpointRunningAsync()
        {
            try
            {
                var streamingEndpointName = AMSConstants.StreamingEndpointName;
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
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw new ExternalAMSErrorException("Could not ensure that streaming endpoint is running", e);
            }
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
            try
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
                        Name = AMSConstants.ContentKeyPolicyOptionName,
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

                return await amsClient.ContentKeyPolicies.CreateOrUpdateAsync(_config.ResourceGroup, _config.AccountName, AMSConstants.ContentKeyPolicyName, options).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw new ExternalAMSErrorException("Could not ensure that content key policy exists", e);
            }
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

            try
            {
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
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw new ExternalAMSErrorException("Could not generate upstream details for livestream", e);
            }
        }

        /// <summary>
        /// Gets the <see cref="StreamingLocator"/> identifier key for a <see cref="Vlog"/>.
        /// </summary>
        /// <param name="vlogId">Internal <see cref="Vlog"/> id</param>
        /// <returns>Key</returns>
        public Task<string> GetVlogStreamingLocatorKeyIdentifierAsync(Guid vlogId)
        {
            vlogId.ThrowIfNullOrEmpty();
            return GetStreamingLocatorKeyIdentifierAsync(AMSNameGenerator.VlogStreamingLocatorName(vlogId));
        }

        /// <summary>
        /// Gets the <see cref="StreamingLocator"/> identifier key for a <see cref="Core.Entities.Reaction"/>.
        /// </summary>
        /// <param name="reactionId">Internal <see cref="Core.Entities.Reaction"/> id</param>
        /// <returns>Key</returns>
        public Task<string> GetReactionStreamingLocatorKeyIdentifierAsync(Guid reactionId)
        {
            reactionId.ThrowIfNullOrEmpty();
            return GetStreamingLocatorKeyIdentifierAsync(AMSNameGenerator.ReactionStreamingLocatorName(reactionId));
        }

        /// <summary>
        /// Gets <see cref="StreamingLocator"/> paths for a vlog.
        /// </summary>
        /// <param name="correspondingVlogId">Internal vlog id (can belong to a livestream).</param>
        /// <returns>All streaming locator paths.</returns>
        public Task<IEnumerable<string>> GetVlogStreamingLocatorPathsAsync(Guid correspondingVlogId)
        {
            correspondingVlogId.ThrowIfNullOrEmpty();
            return GetStreamingLocatorPathsAsync(AMSNameGenerator.VlogStreamingLocatorName(correspondingVlogId));
        }

        /// <summary>
        /// Gets <see cref="StreamingLocator"/> paths for a reaction.
        /// </summary>
        /// <param name="reactionId">Internal <see cref="Core.Entities.Reaction"/> id</param>
        /// <returns><see cref="StreamingLocator"/> paths</returns>
        public Task<IEnumerable<string>> GetReactionStreamingLocatorPathsAsync(Guid reactionId)
        {
            reactionId.ThrowIfNullOrEmpty();
            return GetStreamingLocatorPathsAsync(AMSNameGenerator.ReactionStreamingLocatorName(reactionId));
        }

        /// <summary>
        /// Gets the <see cref="StreamingEndpoint"/> host name.
        /// </summary>
        /// <returns>Host name <see cref="Uri"/></returns>
        public async Task<Uri> GetStreamingEndpointHostNameAsync()
        {
            try
            {
                var streamingEndpoint = await amsClient.StreamingEndpoints.GetAsync(_config.ResourceGroup, _config.AccountName, AMSConstants.StreamingEndpointName).ConfigureAwait(false);
                return new UriBuilder
                {
                    Scheme = "https",
                    Host = streamingEndpoint.HostName
                }.Uri;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw new ExternalAMSErrorException("Could not get host name for streaming endpoint", e);
            }
        }

        /// <summary>
        /// Checks if Azure Media Services is up and running.
        /// </summary>
        /// <remarks>
        /// This just polls all live events.
        /// TODO Enhance in the future
        /// </remarks>
        /// <returns><see cref="bool"/> result</returns>
        public async Task<bool> IsServiceAvailableAsync()
        {
            try
            {
                await amsClient.LiveEvents.ListAsync(_config.ResourceGroup, _config.AccountName).ConfigureAwait(false);
                return true;
            }
            catch (Exception e)
            {
                _logger.LogError("Error while health checking AMS", e.Message);
                return false;
            }
        }

        /// <summary>
        /// Starts a <see cref="LiveEvent"/> in AMS.
        /// </summary>
        /// <param name="liveEventName">AMS <see cref="LiveEvent"/> name</param>
        /// <returns><see cref="Task"/></returns>
        public async Task StartLiveEventAsync(string liveEventName)
        {
            liveEventName.ThrowIfNullOrEmpty();

            try
            {
                await amsClient.LiveEvents.StartAsync(_config.ResourceGroup, _config.AccountName, liveEventName).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw new ExternalAMSErrorException("Could not start live event", e);
            }
        }

        /// <summary>
        /// Stops a <see cref="LiveEvent"/> in AMS.
        /// </summary>
        /// <param name="liveEventName">AMS <see cref="LiveEvent"/> name</param>
        /// <returns><see cref="Task"/></returns>
        public async Task StopLiveEventAsync(string liveEventName)
        {
            liveEventName.ThrowIfNullOrEmpty();

            try
            {
                await amsClient.LiveEvents.StopAsync(_config.ResourceGroup, _config.AccountName, liveEventName).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw new ExternalAMSErrorException("Could not stop live event", e);
            }
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

            try
            {
                await amsClient.LiveOutputs.DeleteAsync(_config.ResourceGroup, _config.AccountName, liveEventName, AMSNameGenerator.VlogLiveOutputName(vlogId)).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw new ExternalAMSErrorException("Could not stop (also meaning delete) live output", e);
            }
        }

        /// <summary>
        /// Creates a new <see cref="Asset"/> in AMS.
        /// </summary>
        /// <param name="assetName">AMS <see cref="Asset"/> name</param>
        /// <returns><see cref="Asset"/></returns>
        private async Task<Asset> CreateAssetAsync(string assetName)
        {
            assetName.ThrowIfNullOrEmpty();

            try
            {
                var assetRequest = new Asset();
                return await amsClient.Assets.CreateOrUpdateAsync(_config.ResourceGroup, _config.AccountName, assetName, assetRequest).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw new ExternalAMSErrorException("Could not create assset", e);
            }
        }

        /// <summary>
        /// Gets a correct RTMPS url from the <see cref="LiveEvent"/>.
        /// </summary>
        /// <param name="liveEvent"></param>
        /// <param name="protocol"></param>
        /// <returns></returns>
        private Uri GetUriFromLiveEvent(LiveEvent liveEvent, string protocol = "RTMP")
        {
            try
            {
                var endpoints = liveEvent.Input.Endpoints;
                if (!endpoints.Any()) { throw new ExternalErrorException("Live Event has no endpoints"); }
                foreach (var endpoint in endpoints)
                {
                    if (endpoint.Protocol.Equals(protocol, StringComparison.InvariantCulture) &&
                        endpoint.Url.Substring(0, 5).Equals("rtmps", StringComparison.InvariantCulture))
                    {
                        return new Uri(endpoint.Url);
                    }
                }
                throw new ExternalAMSErrorException($"Could not find Live Event endpoint for protocol {protocol}");
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw new ExternalAMSErrorException("Could not get Uri from Live Event", e);
            }
        }

        public async Task CreateReactionInputOutputAssetAsync(Guid reactionId)
        {
            reactionId.ThrowIfNullOrEmpty();

            try
            {
                // Create assets
                await amsClient.Assets.CreateOrUpdateAsync(_config.ResourceGroup, _config.AccountName, AMSNameGenerator.ReactionInputAssetName(reactionId), new Asset()).ConfigureAwait(false);
                await amsClient.Assets.CreateOrUpdateAsync(_config.ResourceGroup, _config.AccountName, AMSNameGenerator.ReactionOutputAssetName(reactionId), new Asset()).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw new ExternalAMSErrorException("Could not create reaction input and output asset", e);
            }
        }

        public async Task CreateReactionJobAsync(Guid reactionId, uint reactionLengthMaxInSeconds)
        {
            reactionId.ThrowIfNullOrEmpty();
            if (reactionLengthMaxInSeconds == 0) { throw new ArgumentOutOfRangeException(nameof(reactionLengthMaxInSeconds)); }

            try
            {
                await amsClient.Jobs.CreateAsync(_config.ResourceGroup, _config.AccountName, AMSConstants.ReactionTransformName, AMSNameGenerator.ReactionJobName(reactionId), new Job
                {
                    Input = new JobInputAsset(AMSNameGenerator.ReactionInputAssetName(reactionId),
                        start: new AbsoluteClipTime(TimeSpan.Zero),
                        end: new AbsoluteClipTime(TimeSpan.FromSeconds(reactionLengthMaxInSeconds))),
                    Outputs = new JobOutput[] { new JobOutputAsset(AMSNameGenerator.ReactionOutputAssetName(reactionId)) }
                }).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw new ExternalAMSErrorException("Could not create reaction job", e);
            }
        }

        public async Task<bool> ExistsAssetAsync(string assetName)
        {
            assetName.ThrowIfNullOrEmpty();

            try
            {
                return (await amsClient.Assets.GetAsync(_config.ResourceGroup, _config.AccountName, assetName).ConfigureAwait(false)) != null;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw new ExternalAMSErrorException("Could not check asset existence", e);
            }
        }

        public Task<Uri> GetReactionInputAssetSasAsync(Guid reactionId)
        {
            reactionId.ThrowIfNullOrEmpty();
            return GetAssetSasAsync(AMSNameGenerator.ReactionInputAssetName(reactionId), AssetContainerPermission.ReadWrite, AMSConstants.ReactionSasInputExpireTimeMinutes);
        }

        public Task<Uri> GetReactionOutputAssetSasAsync(Guid reactionId)
        {
            reactionId.ThrowIfNullOrEmpty();
            return GetAssetSasAsync(AMSNameGenerator.ReactionOutputAssetName(reactionId), AssetContainerPermission.Read, AMSConstants.ReactionSasOutputExpireTimeMinutes);
        }

        public Task<Uri> GetVlogOutputAssetSasAysync(Guid vlogId)
        {
            vlogId.ThrowIfNullOrEmpty();
            return GetAssetSasAsync(AMSNameGenerator.VlogLiveOutputAssetName(vlogId), AssetContainerPermission.Read, AMSConstants.VlogSasOutputExpireTimeMinutes);
        }

        public async Task DeleteAssetAsync(string assetName)
        {
            assetName.ThrowIfNullOrEmpty();

            try
            {
                await amsClient.Assets.DeleteAsync(_config.ResourceGroup, _config.AccountName, assetName).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw new ExternalAMSErrorException("Could not delete asset", e);
            }
        }

        public async Task DeleteReactionJobAsync(Guid reactionId)
        {
            reactionId.ThrowIfNullOrEmpty();

            try
            {
                await amsClient.Jobs.DeleteAsync(_config.ResourceGroup, _config.AccountName, AMSConstants.ReactionTransformName, AMSNameGenerator.ReactionJobName(reactionId)).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw new ExternalAMSErrorException("Could not delete reaction job", e);
            }
        }

        /// <summary>
        /// Creates a new instance of <see cref="AzureMediaServicesClient"/>.
        /// </summary>
        /// <param name="config"><see cref="AMSConfiguration"/></param>
        /// <returns><see cref="AzureMediaServicesClient"/></returns>
        private async Task<IAzureMediaServicesClient> BuildClientAsync()
        {
            var clientCredential = new ClientCredential(_config.AadClientId, _config.AadSecret);
            var credentials = await ApplicationTokenProvider.LoginSilentAsync(_config.AadTenantId, clientCredential, ActiveDirectoryServiceSettings.Azure).ConfigureAwait(false);

            return new AzureMediaServicesClient(_config.ArmEndpoint, credentials)
            {
                SubscriptionId = _config.SubscriptionId,
            };
        }

        private async Task<Uri> GetAssetSasAsync(string assetName, AssetContainerPermission permissions, uint expireTimeInMinutes)
        {
            assetName.ThrowIfNullOrEmpty();
            if (expireTimeInMinutes == 0) { throw new ArgumentOutOfRangeException(nameof(expireTimeInMinutes)); }

            try
            {
                return new Uri((await amsClient.Assets.ListContainerSasAsync(
                    _config.ResourceGroup,
                    _config.AccountName,
                    assetName,
                    permissions: permissions,
                    expiryTime: DateTime.UtcNow.AddMinutes(expireTimeInMinutes).ToUniversalTime())
                        .ConfigureAwait(false))
                        .AssetContainerSasUrls
                        .First()); // First key
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw new ExternalAMSErrorException("Could not get asset sas uri", e);
            }
        }

        /// <summary>
        /// Gets the <see cref="StreamingLocator"/> identifier key for a <see cref="StreamingLocator"/>.
        /// </summary>
        /// <param name="streamingLocatorName">AMS <see cref="StreamingLocator"/> name</param>
        /// <returns>Key</returns>
        private async Task<string> GetStreamingLocatorKeyIdentifierAsync(string streamingLocatorName)
        {
            streamingLocatorName.ThrowIfNullOrEmpty();

            try
            {
                var streamingLocator = await amsClient.StreamingLocators.GetAsync(_config.ResourceGroup, _config.AccountName, streamingLocatorName).ConfigureAwait(false);
                return streamingLocator.ContentKeys.First().Id.ToString();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw new ExternalAMSErrorException("Could not get streaming locator key identifier", e);
            }
        }

        private async Task<IEnumerable<string>> GetStreamingLocatorPathsAsync(string streamingLocatorName)
        {
            streamingLocatorName.ThrowIfNullOrEmpty();

            try
            {
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
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw new ExternalAMSErrorException("Could not get streaming locator paths", e);
            }
        }

        /// <summary>
        ///     Called on graceful shutdown.
        /// </summary>
        public void Dispose() => amsClient.Dispose();
    }
}
