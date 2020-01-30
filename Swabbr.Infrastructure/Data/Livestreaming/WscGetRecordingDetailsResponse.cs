using Newtonsoft.Json;
using System.Collections.Generic;

namespace Swabbr.Infrastructure.Data.Livestreaming
{
    public partial class WscGetRecordingDetailsResponse
    {
        [JsonProperty("recordings")]
        public List<WscRecordingDetails> Recordings { get; set; }
    }
}