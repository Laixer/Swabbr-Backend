using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Swabbr.Infrastructure.Data.Livestreaming
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
