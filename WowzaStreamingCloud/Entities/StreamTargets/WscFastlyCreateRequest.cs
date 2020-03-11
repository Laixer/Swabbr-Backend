using Newtonsoft.Json;

namespace Swabbr.WowzaStreamingCloud.Entities.StreamTargets
{

    /// <summary>
    /// Wrapper for creating a new Fastly stream target.
    /// </summary>
    public sealed class WscFastlyCreateRequest
    {

        [JsonProperty("stream_target_fastly")]
        public SubWscFastlyStreamTarget StreamTargetFastly { get; set; }

    }

    public sealed class SubWscFastlyStreamTarget
    {

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("force_ssl_playback")]
        public bool ForceSslPlayback { get; set; }

        [JsonProperty("token_auth_enabled")]
        public bool TokenAuthEnabled { get; set; }

    }

}
