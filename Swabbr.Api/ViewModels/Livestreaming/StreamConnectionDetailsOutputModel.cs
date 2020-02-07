using Newtonsoft.Json;

namespace Swabbr.Api.ViewModels
{
    public class StreamConnectionDetailsOutputModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("host_address")]
        public string HostAddress { get; set; }

        [JsonProperty("app_name")]
        public string AppName { get; set; }

        [JsonProperty("stream_name")]
        public string StreamName { get; set; }

        [JsonProperty("port")]
        public ushort Port { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }
    }
}