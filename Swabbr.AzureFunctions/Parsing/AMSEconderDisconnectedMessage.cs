namespace Swabbr.AzureFunctions.Parsing
{

    /// <summary>
    /// JSON wrapper for the event message when a user disconnects from a live event.
    /// </summary>
    public sealed class AMSEncoderDisconnectedMessage
    {

        public string StreamId { get; set; }

        public string IngestUrl { get; set; }

        public string EncoderIp { get; set; }

        public string EncoderPort { get; set; }

        public string ResultCode { get; set; }

    }

}
