using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WowzaStreamingCloud.Configuration;
using WowzaStreamingCloud.Data;

namespace WowzaStreamingCloud
{

    // TODO THOMAS There is no error checking anywhere in this file
    public sealed class WowzaClient : IDisposable
    {

        private readonly HttpClient HttpClient = new HttpClient();
        private readonly WowzaStreamingCloudConfiguration WscOptions;
        public Uri ApiBase { get;}

        public WowzaClient(WowzaStreamingCloudConfiguration options, Uri apiBase)
        {
            WscOptions = options;
            ApiBase = apiBase;
        }

        public WowzaClient(WowzaStreamingCloudConfiguration options) 
            : this(options, new Uri($"{options.Host}/api/{options.Version}/"))
        {

        }

        // TODO THOMAS We don't need this wrapper if we always want JSON anyways.
        // TODO THOMAS This should be in a separate class, since it is 100% network functionality
        private async Task<TReturn> SendHttpRequestAsync<TReturn>(HttpMethod method, Uri requestUrl, object data = null, string mediaType = "application/json")
            where TReturn : new()
        {
            var response = await SendHttpRequestAsync(method, requestUrl, data, mediaType);
            return Deserialize<TReturn>(response);
        }

        private async Task<string> SendHttpRequestAsync(HttpMethod method, Uri requestUrl, object data = null, string mediaType = "application/json")
        {
            // TODO THOMAS Mediatype can be anything, this is dangerous --> use enum
            string ConvertData(object obj)
            {
                return obj == null ? string.Empty : Serialize(obj); // TODO THOMAS Using an empty string seems wrong here --> doc wants this, +check this! (yorick)
            }

            using (var requestMessage = new HttpRequestMessage(method, requestUrl))
            {
                requestMessage.Headers.Add("wsc-api-key", WscOptions.ApiKey);
                requestMessage.Headers.Add("wsc-access-key", WscOptions.AccessKey);

                // TODO THOMAS I'm not sure if this is the correct method (could be), worth checking --> yorick: misschien de stream teruggeven, even uitzoeken
                using (var stringContent = new StringContent(ConvertData(data), Encoding.UTF8, mediaType))
                {
                    requestMessage.Content = stringContent;
                    var result = await HttpClient.SendAsync(requestMessage);
                    result.EnsureSuccessStatusCode();
                    
                    var resultString = await result.Content.ReadAsStringAsync();
                    return resultString;
                }
            }
        }

        public async Task<WscCreateLivestreamResponse> CreateStream(WscCreateLivestreamRequest request)
        {
            Uri uri = new Uri(ApiBase, "live_streams");
            return await SendHttpRequestAsync<WscCreateLivestreamResponse>(HttpMethod.Post, uri, request);
        }

        public async Task DeleteStreamAsync(string id)
        {
            Uri uri = new Uri(ApiBase, $"live_streams/{id}");
            await SendHttpRequestAsync(HttpMethod.Delete, uri);
        }

        public async Task<WscGetLivestreamResponse> GetStreamAsync(string id)
        {
            Uri uri = new Uri(ApiBase, $"live_streams/{id}");
            return await SendHttpRequestAsync<WscGetLivestreamResponse>(HttpMethod.Get, uri);
        }

        public async Task StartStreamAsync(string id)
        {
            Uri uri = new Uri(ApiBase, $"live_streams/{id}/start");
            await SendHttpRequestAsync(HttpMethod.Put, uri);
        }

        public async Task StopStreamAsync(string id)
        {
            Uri uri = new Uri(ApiBase, $"live_streams/{id}/stop");
            await SendHttpRequestAsync(HttpMethod.Put, uri);
        }

        public async Task<WscGetRecordingDetailsResponse> GetRecordingsAsync(string id)
        {
            Uri uri = new Uri(ApiBase, $"transcoders/{id}/recordings");
            return await SendHttpRequestAsync<WscGetRecordingDetailsResponse>(HttpMethod.Get, uri);
        }

        public async Task<WscGetThumbnailResponse> GetThumbnailUrlAsync(string id)
        {
            Uri uri = new Uri(ApiBase, $"live_streams/{id}/thumbnail_url");
            return await SendHttpRequestAsync<WscGetThumbnailResponse>(HttpMethod.Get, uri);
        }

        public string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public T Deserialize<T>(string str) where T : new()
        {
            return JsonConvert.DeserializeObject<T>(str);
        }

        public void Dispose()
        {
            HttpClient.Dispose();
        }
    }
}
