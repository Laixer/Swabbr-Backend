using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace WowzaStreamingCloud.Data
{
    /// <summary>
    /// Wowza Streaming Cloud API Version 1.3 Livestream object
    /// </summary>
    public partial class WscLivestream
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("transcoder_type")]
        public string TranscoderType { get; set; }

        [JsonProperty("billing_mode")]
        public string BillingMode { get; set; }

        [JsonProperty("broadcast_location")]
        public string BroadcastLocation { get; set; }

        [JsonProperty("recording")]
        public bool Recording { get; set; }

        [JsonProperty("closed_caption_type")]
        public string ClosedCaptionType { get; set; }

        [JsonProperty("low_latency")]
        public bool LowLatency { get; set; }

        [JsonProperty("encoder")]
        public string Encoder { get; set; }

        [JsonProperty("delivery_method")]
        public string DeliveryMethod { get; set; }

        [JsonProperty("target_delivery_protocol")]
        public string TargetDeliveryProtocol { get; set; }

        [JsonProperty("use_stream_source")]
        public bool UseStreamSource { get; set; }

        [JsonProperty("aspect_ratio_width")]
        public long AspectRatioWidth { get; set; }

        [JsonProperty("aspect_ratio_height")]
        public long AspectRatioHeight { get; set; }

        [JsonProperty("connection_code")]
        public string ConnectionCode { get; set; }

        [JsonProperty("connection_code_expires_at")]
        public DateTimeOffset ConnectionCodeExpiresAt { get; set; }

        [JsonProperty("delivery_protocols")]
        public List<string> DeliveryProtocols { get; set; }

        [JsonProperty("source_connection_information")]
        public SourceConnectionInformation SourceConnectionInformation { get; set; }

        [JsonProperty("player_id")]
        public string PlayerId { get; set; }

        [JsonProperty("player_type")]
        public string PlayerType { get; set; }

        [JsonProperty("player_responsive")]
        public bool PlayerResponsive { get; set; }

        [JsonProperty("player_countdown")]
        public bool PlayerCountdown { get; set; }

        [JsonProperty("player_embed_code")]
        public string PlayerEmbedCode { get; set; }

        [JsonProperty("player_hls_playback_url")]
        public Uri PlayerHlsPlaybackUrl { get; set; }

        [JsonProperty("hosted_page")]
        public bool HostedPage { get; set; }

        [JsonProperty("hosted_page_title")]
        public string HostedPageTitle { get; set; }

        [JsonProperty("hosted_page_url")]
        public string HostedPageUrl { get; set; }

        [JsonProperty("hosted_page_sharing_icons")]
        public bool HostedPageSharingIcons { get; set; }

        [JsonProperty("stream_targets")]
        public List<StreamTarget> StreamTargets { get; set; }

        [JsonProperty("direct_playback_urls")]
        public DirectPlaybackUrls DirectPlaybackUrls { get; set; }

        [JsonProperty("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public DateTimeOffset UpdatedAt { get; set; }
    }

    public partial class DirectPlaybackUrls
    {
        [JsonProperty("rtmp")]
        public List<Rtmp> Rtmp { get; set; }

        [JsonProperty("rtsp")]
        public List<Rtmp> Rtsp { get; set; }

        [JsonProperty("wowz")]
        public List<Rtmp> Wowz { get; set; }
    }

    public partial class Rtmp
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("output_id", NullValueHandling = NullValueHandling.Ignore)]
        public string OutputId { get; set; }
    }

    public partial class SourceConnectionInformation
    {
        [JsonProperty("primary_server")]
        public string PrimaryServer { get; set; }

        [JsonProperty("host_port")]
        public long HostPort { get; set; }

        [JsonProperty("application")]
        public string Application { get; set; }

        [JsonProperty("stream_name")]
        public string StreamName { get; set; }

        [JsonProperty("disable_authentication")]
        public bool DisableAuthentication { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }
    }

    public partial class StreamTarget
    {
        [JsonProperty("id")]
        public string Id { get; set; }
    }
}