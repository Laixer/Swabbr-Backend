using Newtonsoft.Json;
using System.Collections.Generic;

namespace Swabbr.WowzaStreamingCloud.Entities
{
    internal partial class WscGetRecordingDetailsResponse
    {
        [JsonProperty("recordings")]
        public List<WscRecordingDetails> Recordings { get; set; }
    }
}