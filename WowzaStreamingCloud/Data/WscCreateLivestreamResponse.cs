using Newtonsoft.Json;

namespace WowzaStreamingCloud.Data
{
    /// <summary>
    /// Wowza Streaming Cloud API Version 1.3 Livestream Create Response
    /// </summary>
    public partial class WscCreateLivestreamResponse
    {
        [JsonProperty("live_stream")]
        public WscLivestream Livestream { get; set; }
    }
}