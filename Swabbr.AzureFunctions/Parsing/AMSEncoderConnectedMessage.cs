namespace Swabbr.AzureFunctions.Parsing
{

    /// <summary>
    /// JSON wrapper for an event where a user starts streaming to a live event.
    /// </summary>
    public sealed class AMSEncoderConnectedMessage
    {

        public string StreamId { get; set; }

        public string IngestUrl { get; set; }

        public string EncoderIp { get; set; }

        public string EncoderPort { get; set; }

    }

}
