using Newtonsoft.Json;

namespace Swabbr.WowzaStreamingCloud.Entities.Livestreams
{
    /// <summary>
    /// Wowza Streaming Cloud API Version 1.3 Livestream Create Response
    /// </summary>
    internal partial class WscCreateLivestreamResponse
    {
        [JsonProperty("live_stream")]
        public WscLivestream Livestream { get; set; }
    }
}