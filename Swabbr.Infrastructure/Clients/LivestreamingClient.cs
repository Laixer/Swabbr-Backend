using Microsoft.Extensions.Options;
using Swabbr.Core.Entities;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Interfaces;
using Swabbr.Core.Interfaces.Clients;
using System;
using System.Threading.Tasks;
using WowzaStreamingCloud;
using WowzaStreamingCloud.Configuration;
using WowzaStreamingCloud.Data;

namespace Swabbr.Infrastructure.Services
{
    public class LivestreamingClient : ILivestreamingClient
    {
        // TODO THOMAS Why static? Just use singleton then
        private static WowzaClient _wowzaClient;

        private readonly ILivestreamRepository _livestreamRepository;

        private readonly WowzaStreamingCloudConfiguration _wscOptions;

        // TODO THOMAS These should be stored in some configuration file
        private const string BillingMode = "pay_as_you_go",
                             ClosedCaptionType = "none",
                             DeliveryMethod = "push",
                             Encoder = "wowza_gocoder",
                             PlayerType = "wowza_player",
                             TranscoderType = "transcoded";

        private const bool
                    HostedPage = true,
                    HostedPageSharingIcons = true,
                    LowLatency = true,
                    PlayerResponsive = true,
                    Recording = true;

        public LivestreamingClient(IOptions<WowzaStreamingCloudConfiguration> wowzaStreamingCloudConfigurationOptions, ILivestreamRepository livestreamRepository, IVlogRepository vlogRepository)
        {
            _wscOptions = wowzaStreamingCloudConfigurationOptions.Value;
            _wowzaClient = new WowzaClient(_wscOptions);
        }

        public async Task<Livestream> CreateNewStreamAsync(string name)
        {
            var createRequest = new WscCreateLivestreamRequest()
            {
                Livestream = new WscCreateLiveStreamRequestBody
                {
                    AspectRatioWidth = _wscOptions.AspectRatioWidth,
                    AspectRatioHeight = _wscOptions.AspectRatioHeight,

                    //TODO: Determine broadcast location based on user location?
                    BroadcastLocation = _wscOptions.BroadcastLocation,

                    Name = name,

                    BillingMode = BillingMode,
                    ClosedCaptionType = ClosedCaptionType,
                    DeliveryMethod = DeliveryMethod,
                    Encoder = Encoder,
                    PlayerType = PlayerType,
                    TranscoderType = TranscoderType,
                    HostedPage = HostedPage,
                    HostedPageSharingIcons = HostedPageSharingIcons,
                    LowLatency = LowLatency,
                    PlayerResponsive = PlayerResponsive,
                    Recording = Recording,
                }
            };

            try
            {
                var response = await _wowzaClient.CreateStream(createRequest);

                // Save the livestream in the database storage
                var createdStream = await _livestreamRepository.CreateAsync(new Livestream
                {
                    Id = response.Livestream.Id, // TODO THOMAS Bad idea to use the external id as the internal id
                    IsActive = false,
                    BroadcastLocation = response.Livestream.BroadcastLocation,
                    CreatedAt = response.Livestream.CreatedAt,
                    Name = response.Livestream.Name,
                    UpdatedAt = response.Livestream.UpdatedAt
                });

                return createdStream;
            }
            catch
            {
                throw new ExternalErrorException("Could not create a new WSC livestream");
            }
        }

        public async Task DeleteStreamAsync(string id)
        {
            await _wowzaClient.DeleteStreamAsync(id);
        }

        // TODO THOMAS This is very bug sensitive, should be transactional, should be completely revisited
        // TODO THOMAS This is a duplicate
        public async Task<StreamConnectionDetails> ReserveLiveStreamForUserAsync(Guid userId)
        {
            int availableStreamCount = await _livestreamRepository.GetAvailableLivestreamCountAsync();
            
            // If no streams are available, create a new stream. 
            //TODO: Minimum amount/lower boundary needed instead of 0?
            if (availableStreamCount < 1)
            {
                await CreateNewStreamAsync("test");
            }

            // Check if there are available (unreserved) livestreams in storage.
            var availableLivestream = await _livestreamRepository.ReserveLivestreamForUserAsync(userId);

            // Retrieve live stream connection details from the api.
            var connection = await GetStreamConnectionAsync(availableLivestream.Id);
            return connection;
        }

        public async Task<StreamConnectionDetails> GetStreamConnectionAsync(string id)
        {
            var response = await _wowzaClient.GetStreamAsync(id);

            // Return the connection details extracted from the received object
            return new StreamConnectionDetails
            {
                Id = response.Livestream.Id,
                AppName = response.Livestream.SourceConnectionInformation.Application,
                HostAddress = response.Livestream.SourceConnectionInformation.PrimaryServer,
                Port = (ushort)response.Livestream.SourceConnectionInformation.HostPort,
                StreamName = response.Livestream.SourceConnectionInformation.StreamName,

                Username = response.Livestream.SourceConnectionInformation.Username,
                Password = response.Livestream.SourceConnectionInformation.Password,
            };
        }

        public async Task<LivestreamPlaybackDetails> GetStreamPlaybackAsync(string id)
        {
            var response = await _wowzaClient.GetStreamAsync(id);

            // Return the playback details extracted from the received object
            return new LivestreamPlaybackDetails
            {
                PlaybackUrl = response.Livestream.PlayerHlsPlaybackUrl
            };
        }

        public async Task StartStreamAsync(string id)
        {
            await _wowzaClient.StartStreamAsync(id);
        }

        public async Task StopStreamAsync(string id)
        {
            await _wowzaClient.StopStreamAsync(id);
        }

        public async Task<string> GetThumbnailUrlAsync(string id)
        {
            var response = await _wowzaClient.GetThumbnailUrlAsync(id);
            return response.Livestream.ThumbnailUrl;
        }

        // TODO THOMAS This doesn't return anything
        public async Task GetRecordingsAsync(string livestreamId)
        {
            //TODO: Poll the recording state and when completed, store the livestream recordings for the specified vlog
            var recordings = await _wowzaClient.GetRecordingsAsync(livestreamId);
            throw new NotImplementedException();
        }

    }
}