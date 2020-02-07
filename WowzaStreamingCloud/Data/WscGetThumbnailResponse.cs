using Newtonsoft.Json;

namespace WowzaStreamingCloud.Data
{
    public class WscGetThumbnailResponse
    {
        [JsonProperty("live_stream")]
        public WscThumbnailContainer Livestream { get; set; }
    }

    public class WscThumbnailContainer
    {
        [JsonProperty("thumbnail_url")]
        public string ThumbnailUrl { get; set; }
    }
}