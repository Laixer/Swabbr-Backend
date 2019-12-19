using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Swabbr.Core.Entities;
using Swabbr.Core.Interfaces;
using Swabbr.Core.Livestreaming;
using Swabbr.Infrastructure.Configuration;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Swabbr.Infrastructure.Services
{
    public class LivestreamingService : ILivestreamingService
    {
        private static readonly HttpClient HttpClient = new HttpClient();

        private readonly WowzaStreamingCloudConfiguration config;

        public LivestreamingService(IOptions<WowzaStreamingCloudConfiguration> options)
        {
            config = options.Value;
        }

        public async Task<StreamConnectionDetails> CreateNewStreamAsync(string name)
        {
            var x = new WcsCreateLivestreamRequest()
            {
                Livestream = new WcsCreateLivestreamRequest.LivestreamInputBody
                {
                    AspectRatioWidth = 1920,
                    AspectRatioHeight = 1080,
                    BroadcastLocation = "eu_belgium",
                    BillingMode = "pay_as_you_go",
                    ClosedCaptionType = "none",
                    DeliveryMethod = "push",
                    Encoder = "wowza_gocoder",
                    HostedPage = true,
                    HostedPageSharingIcons = true,
                    Name = name,
                    PlayerResponsive = true,
                    PlayerType = "wowza_player",
                    TranscoderType = "transcoded"
                }
            };

            var json = JsonConvert.SerializeObject(x);
            using (var stringContent = new StringContent(json, Encoding.UTF8, "application/json"))
            {
                stringContent.Headers.Add("wsc-api-key", config.ApiKey);
                stringContent.Headers.Add("wsc-access-key", config.AccessKey);
                var result = await HttpClient.PostAsync($"{config.Host}/api/{config.Version}/live_streams", stringContent);
                var resultString = await result.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<WcsCreateLivestreamResponse>(resultString);
                
                return new StreamConnectionDetails
                {
                    AppName = response.Livestream.SourceConnectionInformation.Application,
                    HostAddress = response.Livestream.SourceConnectionInformation.PrimaryServer,
                    Port = response.Livestream.SourceConnectionInformation.HostPort.ToString(),
                    StreamName = response.Livestream.SourceConnectionInformation.StreamName,

                    Username = response.Livestream.SourceConnectionInformation.Username,
                    Password = response.Livestream.SourceConnectionInformation.Password,
                };
            };
        }

        public async Task<StreamConnectionDetails> GetStreamAsync(string id)
        {
            var requestUrl = $"{config.Host}/api/{config.Version}/live_streams/{id}";

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUrl))
            {
                requestMessage.Headers.Add("wsc-api-key", config.ApiKey);
                requestMessage.Headers.Add("wsc-access-key", config.AccessKey);

                var result = await HttpClient.SendAsync(requestMessage);
                var resultString = await result.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<WcsGetLivestreamResponse>(resultString);

                // Return the connection details extracted from the received object
                return new StreamConnectionDetails
                {
                    AppName = response.Livestream.SourceConnectionInformation.Application,
                    HostAddress = response.Livestream.SourceConnectionInformation.PrimaryServer,
                    Port = response.Livestream.SourceConnectionInformation.HostPort.ToString(),
                    StreamName = response.Livestream.SourceConnectionInformation.StreamName,
                    
                    Username = response.Livestream.SourceConnectionInformation.Username,
                    Password = response.Livestream.SourceConnectionInformation.Password,
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
                stringContent.Headers.Add("wsc-api-key", config.ApiKey);
                stringContent.Headers.Add("wsc-access-key", config.AccessKey);

                var result = await HttpClient.PutAsync($"{config.Host}/api/{config.Version}/live_streams/{id}/start", stringContent);

                var obj = JsonConvert.DeserializeObject<JObject>(await result.Content.ReadAsStringAsync());
            }
        }

        public async Task StopStreamAsync(string id)
        {
            using (var stringContent = new StringContent(string.Empty, Encoding.UTF8, "application/json"))
            {
                stringContent.Headers.Add("wsc-api-key", config.ApiKey);
                stringContent.Headers.Add("wsc-access-key", config.AccessKey);

                var result = await HttpClient.PutAsync($"{config.Host}/api/{config.Version}/live_streams/{id}/stop", stringContent);
            }
        }

        public async Task<JObject> UpdateStreamAsync(string id)
        {
            throw new System.NotImplementedException();
        }
    }
}