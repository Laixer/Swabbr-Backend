using Newtonsoft.Json;

namespace Swabbr.WowzaStreamingCloud.Entities
{
    internal class WscGetSingleRecordingResponse
    {
        [JsonProperty("recording")]
        public WscRecording Recording { get; set; }
    }
}