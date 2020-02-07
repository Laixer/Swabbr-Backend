using Newtonsoft.Json;

namespace WowzaStreamingCloud.Data
{
    public class WscGetSingleRecordingResponse
    {
        [JsonProperty("recording")]
        public WscRecording Recording { get; set; }
    }
}