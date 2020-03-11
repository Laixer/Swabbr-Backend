using Laixer.Utility.Extensions;
using Newtonsoft.Json;
using Swabbr.WowzaStreamingCloud.Enums;

namespace Swabbr.WowzaStreamingCloud.Entities.Outputs
{

    /// <summary>
    /// Wrapper for creating a new transcoder output.
    /// </summary>
    public sealed class WscOutputRequest
    {

        [JsonProperty("output")]
        public SubWscTranscoderOutputRequest Output { get; set; }

    }

    /// <summary>
    /// TODO These values should come from config
    /// </summary>
    public sealed class SubWscTranscoderOutputRequest
    {

        // TODO Clean up
        //public WscStreamFormat StreamFormat { get; set; } = WscStreamFormat.AudioVideo;
        [JsonProperty("stream_format")]
        public string StreamFormat { get; set; } = WscStreamFormat.AudioVideo.GetEnumMemberAttribute();

        [JsonProperty("bitrate_audio")]
        public int BitrateAudio { get; set; } = 256;

        [JsonProperty("bitrate_video")]
        public int BitrateVideo { get; set; } = 4000;

        [JsonProperty("aspect_ratio_width")]
        public int AspectRatioWidth { get; set; } = 1280;

        [JsonProperty("aspect_ratio_height")]
        public int AspectRatioHeight { get; set; } = 720;

    }

}
