using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Swabbr.Core.Entities;
using Swabbr.Core.Exceptions;
using Swabbr.Core.Interfaces;
using Swabbr.Infrastructure.Configuration;
using Swabbr.Infrastructure.Data.Livestreaming;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Swabbr.Infrastructure.Services
{
    public class LivestreamingService : ILivestreamingService
    {
        private static readonly HttpClient HttpClient = new HttpClient();
        private readonly ILivestreamRepository _livestreamRepository;
        private readonly IVlogRepository _vlogRepository;

        private readonly WowzaStreamingCloudConfiguration wscConfig;

        public LivestreamingService(IOptions<WowzaStreamingCloudConfiguration> options, ILivestreamRepository livestreamRepository, IVlogRepository vlogRepository)
        {
            wscConfig = options.Value;
            _livestreamRepository = livestreamRepository;
            _vlogRepository = vlogRepository;
        }

        public async Task<Livestream> CreateNewStreamAsync(string name)
        {
            var x = new WscCreateLivestreamRequest()
            {
                Livestream = new WcsCreateLiveStreamRequestBody
                {
                    AspectRatioWidth = wscConfig.AspectRatioWidth,
                    AspectRatioHeight = wscConfig.AspectRatioHeight,

                    // TODO Determine broadcast location based on user location?
                    BroadcastLocation = wscConfig.BroadcastLocation,

                    BillingMode = "pay_as_you_go",
                    ClosedCaptionType = "none",
                    DeliveryMethod = "push",
                    Encoder = "wowza_gocoder",
                    HostedPage = true,
                    HostedPageSharingIcons = true,
                    LowLatency = true,
                    Name = name,
                    PlayerResponsive = true,
                    PlayerType = "wowza_player",
                    Recording = true,
                    TranscoderType = "transcoded"
                }
            };

            var json = JsonConvert.SerializeObject(x);

            using (var stringContent = new StringContent(json, Encoding.UTF8, "application/json"))
            {
                stringContent.Headers.Add("wsc-api-key", wscConfig.ApiKey);
                stringContent.Headers.Add("wsc-access-key", wscConfig.AccessKey);

                var result = await HttpClient.PostAsync($"{wscConfig.Host}/api/{wscConfig.Version}/live_streams", stringContent);

                if (result.IsSuccessStatusCode)
                {
                    var resultString = await result.Content.ReadAsStringAsync();
                    var response = JsonConvert.DeserializeObject<WscCreateLivestreamResponse>(resultString);

                    // Save the livestream in the database storage
                    var createdStream = await _livestreamRepository.CreateAsync(new Livestream
                    {
                        Id = response.Livestream.Id,
                        IsActive = false,
                        BroadcastLocation = response.Livestream.BroadcastLocation,
                        CreatedAt = response.Livestream.CreatedAt,
                        Name = response.Livestream.Name,
                        UpdatedAt = response.Livestream.UpdatedAt
                    });

                    return createdStream;
                }

                throw new ExternalErrorException("Could not create a new WSC livestream");
            };
        }

        public async Task DeleteStreamAsync(string id)
        {
            var requestUrl = $"{wscConfig.Host}/api/{wscConfig.Version}/live_streams/{id}";

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Delete, requestUrl))
            {
                requestMessage.Headers.Add("wsc-api-key", wscConfig.ApiKey);
                requestMessage.Headers.Add("wsc-access-key", wscConfig.AccessKey);

                var result = await HttpClient.SendAsync(requestMessage);
            }
        }

        public async Task<StreamConnectionDetails> ReserveLiveStreamForUserAsync(Guid userId)
        {
            try
            {
                // Check if there are available (unreserved) livestreams in storage.
                var availableLivestream = await _livestreamRepository.ReserveLivestreamForUserAsync(userId);

                // Retrieve live stream connection details from the api.
                var connection = await GetStreamConnectionAsync(availableLivestream.Id);
                return connection;
            }
            catch (EntityNotFoundException)
            {
                // There were no available livestreams.

                // Create a new livestream and add it to the pool and return its connection details.
                // TODO How to determine the name of the newly created stream?
                var newStream = await CreateNewStreamAsync("test");

                if (newStream != null)
                {
                    // Try to reserve a livestream again after having created the new stream.
                    var availableLivestream = await _livestreamRepository.ReserveLivestreamForUserAsync(userId);
                    var connection = await GetStreamConnectionAsync(availableLivestream.Id);
                    return connection;
                }

                throw new ExternalErrorException("No livestreams available and could not create livestream.");
            }
        }

        public async Task<StreamConnectionDetails> GetStreamConnectionAsync(string id)
        {
            var requestUrl = $"{wscConfig.Host}/api/{wscConfig.Version}/live_streams/{id}";

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUrl))
            {
                requestMessage.Headers.Add("wsc-api-key", wscConfig.ApiKey);
                requestMessage.Headers.Add("wsc-access-key", wscConfig.AccessKey);

                var result = await HttpClient.SendAsync(requestMessage);
                var resultString = await result.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<WscGetLivestreamResponse>(resultString);

                // Return the connection details extracted from the received object
                return new StreamConnectionDetails
                {
                    Id = response.Livestream.Id,
                    AppName = response.Livestream.SourceConnectionInformation.Application,
                    HostAddress = response.Livestream.SourceConnectionInformation.PrimaryServer,
                    Port = response.Livestream.SourceConnectionInformation.HostPort.ToString(),
                    StreamName = response.Livestream.SourceConnectionInformation.StreamName,

                    Username = response.Livestream.SourceConnectionInformation.Username,
                    Password = response.Livestream.SourceConnectionInformation.Password,
                };
            }
        }

        public async Task<StreamPlaybackDetails> GetStreamPlaybackAsync(string id)
        {
            var requestUrl = $"{wscConfig.Host}/api/{wscConfig.Version}/live_streams/{id}";

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUrl))
            {
                requestMessage.Headers.Add("wsc-api-key", wscConfig.ApiKey);
                requestMessage.Headers.Add("wsc-access-key", wscConfig.AccessKey);

                var result = await HttpClient.SendAsync(requestMessage);
                var resultString = await result.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<WscGetLivestreamResponse>(resultString);

                // Return the playback details extracted from the received object
                return new StreamPlaybackDetails
                {
                    PlaybackUrl = response.Livestream.PlayerHlsPlaybackUrl
                };
            }
        }

        public async Task StartStreamAsync(string id)
        {
            using (var stringContent = new StringContent(string.Empty, Encoding.UTF8, "application/json"))
            {
                stringContent.Headers.Add("wsc-api-key", wscConfig.ApiKey);
                stringContent.Headers.Add("wsc-access-key", wscConfig.AccessKey);

                var result = await HttpClient.PutAsync($"{wscConfig.Host}/api/{wscConfig.Version}/live_streams/{id}/start", stringContent);

                var obj = JsonConvert.DeserializeObject<JObject>(await result.Content.ReadAsStringAsync());
            }
        }

        public async Task StopStreamAsync(string id)
        {
            using (var stringContent = new StringContent(string.Empty, Encoding.UTF8, "application/json"))
            {
                stringContent.Headers.Add("wsc-api-key", wscConfig.ApiKey);
                stringContent.Headers.Add("wsc-access-key", wscConfig.AccessKey);

                var result = await HttpClient.PutAsync($"{wscConfig.Host}/api/{wscConfig.Version}/live_streams/{id}/stop", stringContent);
            }
        }

        public async Task<string> GetThumbnailUrlAsync(string id)
        {
            var requestUrl = $"{wscConfig.Host}/api/{wscConfig.Version}/live_streams/{id}/thumbnail_url";

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUrl))
            {
                requestMessage.Headers.Add("wsc-api-key", wscConfig.ApiKey);
                requestMessage.Headers.Add("wsc-access-key", wscConfig.AccessKey);

                var result = await HttpClient.SendAsync(requestMessage);
                var resultString = await result.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<WscGetThumbnailResponse>(resultString);

                // Return the thumbnail url extracted from the received object
                return response.Livestream.ThumbnailUrl;
            }
        }

        public async Task SyncRecordingsForVlogAsync(string livestreamId, Guid vlogId)
        {
            //TODO Determine how long the delay for waiting for recordings should be
            await Task.Delay(TimeSpan.FromMinutes(1));

            var getAllRecordingsUrl = $"{wscConfig.Host}/api/{wscConfig.Version}/transcoders/{livestreamId}/recordings";

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, getAllRecordingsUrl))
            {
                requestMessage.Headers.Add("wsc-api-key", wscConfig.ApiKey);
                requestMessage.Headers.Add("wsc-access-key", wscConfig.AccessKey);

                var result = await HttpClient.SendAsync(requestMessage);
                var resultString = await result.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<WscGetRecordingDetailsResponse>(resultString);

                foreach (WscRecordingDetails recordingDetails in response.Recordings)
                {
                    await SyncRecordingForVlogAsync(recordingDetails, vlogId);
                }
            }
        }

        private async Task SyncRecordingForVlogAsync(WscRecordingDetails recordingDetails, Guid vlogId)
        {
            var requestSingleRecordingUrl = $"{wscConfig.Host}/api/{wscConfig.Version}/recordings/{recordingDetails.Id}";

            WscRecording recording;

            int retryCount = 0;
            TimeSpan retryTimeSpan = TimeSpan.FromSeconds(30);

            do
            {
                using (var requestRecordingMessage = new HttpRequestMessage(HttpMethod.Get, requestSingleRecordingUrl))
                {
                    requestRecordingMessage.Headers.Add("wsc-api-key", wscConfig.ApiKey);
                    requestRecordingMessage.Headers.Add("wsc-access-key", wscConfig.AccessKey);

                    // Fetch latest recording from media service
                    var response = await HttpClient.SendAsync(requestRecordingMessage);
                    var responseString = await response.Content.ReadAsStringAsync();
                    recording = JsonConvert.DeserializeObject<WscGetSingleRecordingResponse>(responseString).Recording;

                    // If the recording
                    if (recording.State.Equals("completed", StringComparison.InvariantCultureIgnoreCase))
                    {
                        // Recording is complete, store the download url in the vlog
                        //TODO Currently overwriting with the latest if there are MULTIPLE recordings. Either store seperately or merge together
                        var vlog = await _vlogRepository.GetByIdAsync(vlogId);
                        vlog.DownloadUrl = recording.DownloadUrl.ToString();
                        await _vlogRepository.UpdateAsync(vlog);
                        break;
                    }
                    else
                    {
                        await Task.Delay(retryTimeSpan);
                        retryCount++;
                    }
                }
            }
            while (
                retryCount < 100
                &&
                !recording.State.Equals("completed", StringComparison.InvariantCultureIgnoreCase)
            );
        }
    }
}