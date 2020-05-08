using Newtonsoft.Json;

namespace Swabbr.Api.ViewModels
{
    public class LivestreamConnectionDetailsOutputModel
    {
        public string ExternalId { get; set; }

        public string HostAddress { get; set; }

        public string AppName { get; set; }

        public string StreamName { get; set; }

        public ushort Port { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }
    }
}