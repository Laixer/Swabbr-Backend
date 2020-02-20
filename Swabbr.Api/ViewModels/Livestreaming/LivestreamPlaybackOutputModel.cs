using Newtonsoft.Json;
using System;

namespace Swabbr.Api.ViewModels
{

    /// <summary>
    /// Represents a livestream playback entity.
    /// </summary>
    public class LivestreamPlaybackOutputModel
    {

        /// <summary>
        /// Playback URL for streaming video
        /// </summary>
        [JsonProperty("playbackUrl")]
        public Uri PlaybackUrl { get; set; }

    }

}
