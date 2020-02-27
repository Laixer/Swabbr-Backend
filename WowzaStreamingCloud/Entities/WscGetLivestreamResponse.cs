using Newtonsoft.Json;

namespace Swabbr.WowzaStreamingCloud.Entities
{
    // Wowza Streaming Cloud API Version 1.3 Livestream response
    internal partial class WscGetLivestreamResponse
    {
        [JsonProperty("live_stream")]
        public WscLivestream Livestream { get; set; }
    }
}