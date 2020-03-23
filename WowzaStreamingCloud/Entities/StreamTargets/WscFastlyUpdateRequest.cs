using Newtonsoft.Json;
using Swabbr.WowzaStreamingCloud.Utility;

namespace Swabbr.WowzaStreamingCloud.Entities.StreamTargets
{

    /// <summary>
    /// Wrapper for creating a new Fastly stream target.
    /// </summary>
    public sealed class WscFastlyUpdateRequest
    {

        [JsonProperty("stream_target_fastly")]
        public SubWscFastlyStreamTargetUpdate StreamTargetFastly { get; set; }

    }

    /// <summary>
    /// Contains all properties required to setup token auth for a fastly 
    /// stream target in the Wowza API.
    /// </summary>
    public sealed class SubWscFastlyStreamTargetUpdate
    {

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("force_ssl_playback")]
        public bool ForceSslPlayback { get; set; }

        [JsonProperty("token_auth_enabled")]
        public bool TokenAuthEnabled { get; set; }

    }

}
