using Newtonsoft.Json;
using System;

namespace Swabbr.Core.Notifications
{
    public class SwabbrMessage
    {
        [JsonProperty("protocol")]
        public string Protocol => "swabbr";

        [JsonProperty("protocol_version")]
        public string ProtocolVersion => "1.0";

        [JsonProperty("data_type")]
        public string DataType { get; set; }

        [JsonProperty("data_type_version")]
        public string DataTypeVersion { get; set; }

        [JsonProperty("data")]
        public object Data { get; set; }

        [JsonProperty("content_type")]
        public string ContentType { get; set; }

        [JsonProperty("timestamp")]
        public DateTime TimeStamp { get; set; }

        [JsonProperty("user_agent")]
        public string UserAgent { get; set; }
    }
}