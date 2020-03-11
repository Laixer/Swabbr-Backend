using Newtonsoft.Json;

namespace Swabbr.WowzaStreamingCloud.Entities.StreamTargets
{

    /// <summary>
    /// Response wrapper for creating a Fastly stream target.
    /// </summary>
    public sealed class WscFastlyCreateResponse
    {

        [JsonProperty("stream_target_fastly")]
        public SubWscFastlyCreateResponse StreamTargetFastly { get; set; }

    }

    public sealed class SubWscFastlyCreateResponse
    {

        /// <summary>
        /// At the moment we only need the id.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

    }

}
