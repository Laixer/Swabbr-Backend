using Newtonsoft.Json;

namespace Swabbr.Infrastructure.Data.Livestreaming
{
    /// <summary>
    /// Wowza Streaming Cloud API Version 1.3 Creating live stream object
    /// </summary>
    public partial class WscCreateLivestreamRequest
    {
        [JsonProperty("live_stream")]
        public WcsCreateLiveStreamRequestBody Livestream { get; set; }
    }

    public partial class WcsCreateLiveStreamRequestBody
    {
        [JsonProperty("aspect_ratio_height")]
        public long AspectRatioHeight { get; set; }

        [JsonProperty("aspect_ratio_width")]
        public long AspectRatioWidth { get; set; }

        [JsonProperty("billing_mode")]
        public string BillingMode { get; set; }

        [JsonProperty("broadcast_location")]
        public string BroadcastLocation { get; set; }

        [JsonProperty("closed_caption_type")]
        public string ClosedCaptionType { get; set; }

        [JsonProperty("delivery_method")]
        public string DeliveryMethod { get; set; }

        [JsonProperty("encoder")]
        public string Encoder { get; set; }

        [JsonProperty("hosted_page")]
        public bool HostedPage { get; set; }

        [JsonProperty("hosted_page_sharing_icons")]
        public bool HostedPageSharingIcons { get; set; }

        [JsonProperty("low_latency")]
        public bool LowLatency { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("player_responsive")]
        public bool PlayerResponsive { get; set; }

        [JsonProperty("player_type")]
        public string PlayerType { get; set; }

        [JsonProperty("recording")]
        public bool Recording { get; set; }

        [JsonProperty("transcoder_type")]
        public string TranscoderType { get; set; }
    }
}