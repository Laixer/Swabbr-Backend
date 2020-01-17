using Newtonsoft.Json;

namespace Swabbr.Core.Entities
{
    public class StreamConnectionDetails
    {
        /// <summary>
        /// Id of the livestream
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Address of the host
        /// </summary>
        public string HostAddress { get; set; }

        /// <summary>
        /// Streaming application name
        /// </summary>
        public string AppName { get; set; }

        /// <summary>
        /// Name of the stream
        /// </summary>
        public string StreamName { get; set; }

        /// <summary>
        /// Port number
        /// </summary>
        public string Port { get; set; }

        /// <summary>
        /// Username to be used for authenticating the connection
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Password to be used for authenticating the connection
        /// </summary>
        public string Password { get; set; }
    }
}