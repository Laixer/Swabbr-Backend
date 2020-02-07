using Newtonsoft.Json;
using System.Collections.Generic;

namespace WowzaStreamingCloud.Data
{
    public partial class WscGetRecordingDetailsResponse
    {
        [JsonProperty("recordings")]
        public List<WscRecordingDetails> Recordings { get; set; }
    }
}