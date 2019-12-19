namespace Swabbr.Core.Livestreaming
{
    using Newtonsoft.Json;

    /// <summary>
    /// Represents a JSON object class for creating a new Wowza Streaming Cloud livestream.
    /// </summary>
    public partial class WcsCreateLivestreamRequest
    {
        [JsonProperty("live_stream")]
        public LivestreamInputBody Livestream { get; set; }

        public partial class LivestreamInputBody
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

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("player_responsive")]
            public bool PlayerResponsive { get; set; }

            [JsonProperty("player_type")]
            public string PlayerType { get; set; }

            [JsonProperty("transcoder_type")]
            public string TranscoderType { get; set; }
        }
    }
}
