using Newtonsoft.Json;
using System.Collections.Generic;

namespace Swabbr.WowzaStreamingCloud.Entities.Outputs
{

    /// <summary>
    /// Response wrapper for GET api/v1.4/transcoders/id/outputs/ from Wowza.
    /// </summary>
    public sealed class WscOutputsResponse
    {

        [JsonProperty("outputs")]
        public IEnumerable<SubWscOutput> Outputs { get; set; }

    }

    /// <summary>
    /// Contains details for a single Wowza Transcoder output.
    /// </summary>
    public sealed class SubWscOutput
    {

        /// <summary>
        /// Output id.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Indicates passthrough of video.
        /// </summary>
        [JsonProperty("passthrough_video")]
        public bool PassthroughVideo { get; set; }

        /// <summary>
        /// Video width
        /// </summary>
        [JsonProperty("aspect_ratio_width")]
        public int AspectRatioWidth { get; set; }

        /// <summary>
        /// Video height.
        /// </summary>
        [JsonProperty("aspect_ratio_height")]
        public int AspectRatioHeight { get; set; }

        /// <summary>
        /// Collection of linked stream targets.
        /// </summary>
        [JsonProperty("output_stream_targets")]
        public IEnumerable<SubWscOutputStreamTarget> OutputStreamTargets { get; set; }

    }

    /// <summary>
    /// Contains details about a single Output Stream Target that is linked to
    /// the Output. 
    /// </summary>
    public sealed class SubWscOutputStreamTarget
    {

        /// <summary>
        /// Not sure what this id is (we don't use it anyways).
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Id of the stream target that is linked to the output.
        /// </summary>
        [JsonProperty("stream_target_id")]
        public string StreamTargetId { get; set; }

        /// <summary>
        /// Contains more details about the stream target.
        /// </summary>
        [JsonProperty("stream_target")]
        public SubWscOutputStreamTargetItem StreamTarget { get; set; }

    }


    /// <summary>
    /// Represents the specific details about a single stream target.
    /// </summary>
    public sealed class SubWscOutputStreamTargetItem
    {

        /// <summary>
        /// Name of this stream target.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Type of this stream target.
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

    }

}
