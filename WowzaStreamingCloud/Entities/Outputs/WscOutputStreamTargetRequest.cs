using Newtonsoft.Json;

namespace Swabbr.WowzaStreamingCloud.Entities.Outputs
{

    /// <summary>
    /// Wrapper for creating a new output stream target.
    /// </summary>
    public sealed class WscOutputStreamTargetRequest
    {

        [JsonProperty("output_stream_target")]
        public SubWscOutputStreamTargetRequest OutputStreamTarget { get; set; }

    }

    public sealed class SubWscOutputStreamTargetRequest
    {

        [JsonProperty("stream_target_id")]
        public string StreamTargetId { get; set; }

    }

}
