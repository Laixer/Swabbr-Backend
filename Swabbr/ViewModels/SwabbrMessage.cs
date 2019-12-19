namespace Swabbr.Api.ViewModels
{
    // TODO Properties
    public abstract class SwabbrMessage
    {
        public string Protocol { get; set; }
        public string ProtocolVersion { get; set; }
        public string DataType { get; set; }
        public string Data { get; set; }
        public string ContentType { get; set; }
        public string Timestamp { get; set; }
    }
}
