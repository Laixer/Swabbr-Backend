using Newtonsoft.Json;

namespace Swabbr.Core.Livestreaming
{
    public partial class WcsGetLivestreamResponse
    {
        [JsonProperty("live_stream")]
        public WcsLivestream Livestream;
    }
}
