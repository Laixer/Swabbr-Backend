using Newtonsoft.Json;

namespace Swabbr.Infrastructure.Data.Livestreaming
{
    // Wowza Streaming Cloud API Version 1.3 Livestream response
    public partial class WscGetLivestreamResponse
    {
        [JsonProperty("live_stream")]
        public WscLivestream Livestream { get; set; }
    }
}
