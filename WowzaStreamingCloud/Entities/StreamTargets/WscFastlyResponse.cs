using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Swabbr.WowzaStreamingCloud.Entities.StreamTargets
{

    /// <summary>
    /// Response wrapper for creating a Fastly stream target.
    /// </summary>
    public sealed class WscFastlyResponse
    {

        [JsonProperty("stream_target_fastly")]
        public SubWscFastlyResponse StreamTargetFastly { get; set; }

    }

    public sealed class SubWscFastlyResponse
    {

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("token_auth_shared_secret")]
        public string TokenAuthSharedSecret { get; set; }

        [JsonProperty("playback_url")]
        public string PlaybackUrl { get; set; }

    }

}
