using Newtonsoft.Json;

namespace Swabbr.Infrastructure.Data.Livestreaming
{
    public partial class WcsGetLivestreamResponse
    {
        [JsonProperty("live_stream")]
        public WcsLivestream Livestream { get; set; }
    }
}
