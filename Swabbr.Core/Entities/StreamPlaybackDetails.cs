using Newtonsoft.Json;
using System;

namespace Swabbr.Core.Entities
{
    public sealed class StreamPlaybackDetails
    {
        [JsonProperty("playbackUrl")]
        public Uri PlaybackUrl { get; set; }
    }
}
