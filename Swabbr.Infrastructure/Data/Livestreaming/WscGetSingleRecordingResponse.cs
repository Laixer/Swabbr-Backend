using Newtonsoft.Json;

namespace Swabbr.Infrastructure.Data.Livestreaming
{
    public class WscGetSingleRecordingResponse
    {
        [JsonProperty("recording")]
        public WscRecording Recording { get; set; }
    }
}
