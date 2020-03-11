namespace Swabbr.WowzaStreamingCloud.Entities.Outputs
{

    /// <summary>
    /// Response wrapper for creating a new transcoder output.
    /// </summary>
    public sealed class WscOutputResponse
    {

        public SubWscTranscoderOutputResponse Output { get; set; }

    }

    public sealed class SubWscTranscoderOutputResponse
    {

        public string Id { get; set; }

    }

}
