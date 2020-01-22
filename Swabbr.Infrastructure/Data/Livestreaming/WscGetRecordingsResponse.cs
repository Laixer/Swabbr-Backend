using Newtonsoft.Json;
using System.Collections.Generic;

namespace Swabbr.Infrastructure.Data.Livestreaming
{
    public partial class WscGetRecordingsResponse
    {
        [JsonProperty("recordings")]
        public List<WscRecording> Recordings { get; set; }
    }
}