using Laixer.Utility.Extensions;
using Microsoft.Azure.Management.Media;
using Microsoft.Azure.Management.Media.Models;
using Microsoft.Extensions.Options;
using Swabbr.AzureMediaServices.Configuration;
using Swabbr.AzureMediaServices.Extensions;
using Swabbr.AzureMediaServices.Utility;
using Swabbr.Core.Entities;
using Swabbr.Core.Enums;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Swabbr.AzureMediaServices.Services
{

    /// <summary>
    /// Handles <see cref="Livestream"/> pool functionality.
    /// </summary>
    public sealed class AMSLivestreamPoolService : ILivestreamPoolService
    {

        private readonly ILivestreamRepository _livestreamRepository;
        private readonly AMSConfiguration _config;

        /// <summary>
        /// Constructor for dependency injection.
        /// </summary>
        public AMSLivestreamPoolService(ILivestreamRepository livestreamRepository,
            IOptions<AMSConfiguration> config)
        {
            _livestreamRepository = livestreamRepository ?? throw new ArgumentNullException(nameof(livestreamRepository));
            _config = config.Value ?? throw new ArgumentNullException(nameof(config.Value));
            _config.ThrowIfInvalid();
        }

        /// <summary>
        /// Creates and sets up a new <see cref="Livestream"/> in Azure Media
        /// Services.
        /// </summary>
        /// <remarks>
        /// TODO Edge case checking & reporting --> max live events per account in AMS
        /// </remarks>
        /// <returns><see cref="Livestream"/></returns>
        public async Task<Livestream> CreateLivestreamAsync()
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                // First create internally (state will be created_internal)
                var livestream = await _livestreamRepository.CreateAsync(new Livestream
                {
                    Name = AMSNameGenerator.LivestreamDefaultName
                }).ConfigureAwait(false);

                // Go through external creation process
                await EnsureLivestreamTransformExistsAsync().ConfigureAwait(false);
                var liveEvent = await CreateLiveEventAsync(livestream.Id).ConfigureAwait(false);

                // Update internally
                livestream.ExternalId = liveEvent.Name;
                livestream.BroadcastLocation = liveEvent.Location;
                await _livestreamRepository.MarkCreatedAsync(livestream.Id, livestream.ExternalId, livestream.BroadcastLocation);

                scope.Complete();
                return livestream;
            };
        }

        /// <summary>
        /// Attempts to get a <see cref="Livestream"/> from the existing pool.
        /// </summary>
        /// <remarks>
        /// This does NOT claim the <see cref="Livestream"/>. Do this using a
        /// transaction (<see cref="TransactionScope"/> is an option).
        /// </remarks>
        /// <returns><see cref="Livestream"/></returns>
        public async Task<Livestream> TryGetLivestreamFromPoolAsync()
        {

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var livestreams = await _livestreamRepository.GetAvailableLivestreamsAsync().ConfigureAwait(false);
                if (!livestreams.Any())
                {
                    // First release FOR UPDATE locks, then attempt new create
                    scope.Complete();
                    return await CreateLivestreamAsync().ConfigureAwait(false);
                }
                else
                {
                    var livestream = livestreams.First();
                    scope.Complete();
                    return livestream;
                }
            }
        }

        /// <summary>
        /// Makes sure that the <see cref="Livestream"/> transform exists on
        /// the Azure Media Services side.
        /// </summary>
        /// <remarks>
        /// TODO This can be optimized, shouldn't need to run every single time.
        /// </remarks>
        /// <returns><see cref="Task"/></returns>
        private async Task EnsureLivestreamTransformExistsAsync()
        {
            var amsClient = await AMSClientFactory.GetClientAsync(_config).ConfigureAwait(false);
            var livestreamTransformName = AMSNameGenerator.LivestreamTransformName;

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

            await amsClient.Transforms.CreateOrUpdateAsync(_config.ResourceGroup, _config.AccountName, livestreamTransformName, outputs, AMSNameGenerator.LivestreamTransformDescription);
        }

        /// <summary>
        /// Creates a new <see cref="LiveEvent"/> in AMS.
        /// </summary>
        /// <remarks>
        /// TODO Maybe centralize these livestream parameters
        /// </remarks>
        /// <param name="livestreamId">Internal <see cref="Livestream"/> id</param>
        /// <returns><see cref="LiveEvent"/></returns>
        private async Task<LiveEvent> CreateLiveEventAsync(Guid livestreamId)
        {
            livestreamId.ThrowIfNullOrEmpty();

            var amsClient = await AMSClientFactory.GetClientAsync(_config).ConfigureAwait(false);
            var liveEventName = AMSNameGenerator.LiveEventName();
            var liveEventRequest = new LiveEvent
            {
                Location = _config.Location,
                Input = new LiveEventInput
                {
                    StreamingProtocol = "RTMP" 
                }
            };
            return await amsClient.LiveEvents.CreateAsync(_config.ResourceGroup, _config.AccountName, liveEventName, liveEventRequest);
        }

    }

}
