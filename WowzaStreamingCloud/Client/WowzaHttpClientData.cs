using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Swabbr.WowzaStreamingCloud.Client
{

    /// <summary>
    /// Contains functionality to communicate and invoke the Wowza API. This is 
    /// a wrapper for the API, which also polls our livestream status when this
    /// is required. This part contains all http and json functionality.
    /// 
    /// TODO Clean this up
    /// </summary>
    internal sealed partial class WowzaHttpClient : IDisposable
    {

        public string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public T Deserialize<T>(string str) where T : new()
        {
            return JsonConvert.DeserializeObject<T>(str);
        }

        // TODO THOMAS We don't need this wrapper if we always want JSON anyways.
        // TODO THOMAS This should be in a separate class, since it is 100% network functionality
        private async Task<TReturn> SendHttpRequestAsync<TReturn>(HttpMethod method, Uri requestUrl, object data = null, string mediaType = "application/json")
            where TReturn : new()
        {
            var response = await SendHttpRequestAsync(method, requestUrl, data, mediaType).ConfigureAwait(false);
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

                    // TODO remove
                    var sendContent = await stringContent.ReadAsStringAsync();

                    requestMessage.Content = stringContent;
                    var result = await httpClient.SendAsync(requestMessage);

                    // TODO REMOVE
                    try
                    {
                        result.EnsureSuccessStatusCode();
                    }
                    catch (Exception)
                    {
                        var content = await result.Content.ReadAsStringAsync();
                        throw;
                    }

                    var resultString = await result.Content.ReadAsStringAsync();
                    return resultString;
                }
            }
        }

    }

}
