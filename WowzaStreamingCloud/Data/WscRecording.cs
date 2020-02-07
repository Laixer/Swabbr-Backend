using Newtonsoft.Json;
using System;

namespace WowzaStreamingCloud.Data
{
    public partial class WscRecording
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("transcoder_id")]
        public string TranscoderId { get; set; }

        [JsonProperty("transcoder_name")]
        public string TranscoderName { get; set; }

        [JsonProperty("starts_at")]
        public DateTimeOffset StartsAt { get; set; }

        [JsonProperty("transcoding_uptime_id")]
        public string TranscodingUptimeId { get; set; }

        [JsonProperty("file_name")]
        public string FileName { get; set; }

        [JsonProperty("file_size")]
        public long FileSize { get; set; }

        [JsonProperty("duration")]
        public long Duration { get; set; }

        [JsonProperty("download_url")]
        public Uri DownloadUrl { get; set; }

        [JsonProperty("reason")]
        public string Reason { get; set; }

        [JsonProperty("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public DateTimeOffset UpdatedAt { get; set; }
    }
}