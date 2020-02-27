using Swabbr.WowzaStreamingCloud.Enums;

namespace Swabbr.WowzaStreamingCloud.Entities
{

    /// <summary>
    /// Response object for the Wowza transcoders/id/stats endpoint.
    /// </summary>
    public sealed class WscTranscoderStats
    {

        public WscTranscoderStatsTranscoder Transcoder { get; set; }

    }

    /// <summary>
    /// Subclass of <see cref="WscTranscoderStats"/>.
    /// </summary>
    public sealed class WscTranscoderStatsTranscoder
    { 
    
        public WscTranscodersStatsTranscoderConnected Connected { get; set; }

    }

    /// <summary>
    /// Subclass of <see cref="WscTranscoderStatsTranscoder"/>.
    /// </summary>
    public sealed class WscTranscodersStatsTranscoderConnected
    {
        
        public WscYesNo Value { get; set; }

    }

}
