using Newtonsoft.Json;

namespace Swabbr.WowzaStreamingCloud.Entities
{
    internal class WscGetThumbnailResponse
    {
        [JsonProperty("live_stream")]
        public WscThumbnailContainer Livestream { get; set; }
    }

    internal class WscThumbnailContainer
    {
        [JsonProperty("thumbnail_url")]
        public string ThumbnailUrl { get; set; }
    }
}