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

        private readonly WowzaStreamingCloudConfiguration wscConfig;

        public LivestreamingService(IOptions<WowzaStreamingCloudConfiguration> options, ILivestreamRepository livestreamRepository)
        {
            wscConfig = options.Value;
            _livestreamRepository = livestreamRepository;
        }

        public async Task<Livestream> CreateNewStreamAsync(string name)
        {
            var x = new WscCreateLivestreamRequest()
            {
                Livestream = new WcsCreateLiveStreamRequestBody
                {
                    // TODO Always use 1080p as default frame resolution?
                    AspectRatioWidth = 1920,
                    AspectRatioHeight = 1080,

                    // TODO Determine broadcast location based on user location
                    BroadcastLocation = "eu_belgium",

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
                        Active = false,
                        BroadcastLocation = response.Livestream.BroadcastLocation,
                        CreatedAt = response.Livestream.CreatedAt,
                        Name = response.Livestream.Name,
                        UpdatedAt = response.Livestream.UpdatedAt
                    });

                    return createdStream;
                }

                return null;
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

        public async Task<StreamConnectionDetails> OpenLiveStreamForUserAsync(Guid userId)
        {
            try
            {
                var availableLivestream = await _livestreamRepository.ReserveLivestreamForUserAsync(userId);

                // Retrieve live stream connection details
                var connection = await GetStreamConnectionAsync(availableLivestream.Id);
                return connection;
            }
            catch (EntityNotFoundException e)
            {
                // Create a new livestream and return it.
                // TODO How to determine the name of the newly created stream?
                var newStream = await CreateNewStreamAsync("testName");

                if (newStream != null)
                {
                    // Try to reserve livestream again after creating the stream
                    var availableLivestream = await _livestreamRepository.ReserveLivestreamForUserAsync(userId);
                    var connection = await GetStreamConnectionAsync(availableLivestream.Id);
                    return connection;
                }

                throw new Exception("No livestreams available and could not create livestream.");
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

        public async Task ResetStreamAsync(string id)
        {
            throw new System.NotImplementedException();
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

        public async Task<JObject> UpdateStreamAsync(string id)
        {
            await Task.FromResult(0);
            throw new NotImplementedException();
        }
    }
}