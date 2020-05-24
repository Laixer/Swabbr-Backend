namespace Swabbr.Core.Types
{

    /// <summary>
    /// Contains all details for a given livestream connection.
    /// </summary>
    public class LivestreamConnectionDetails
    {

        /// <summary>
        /// External id of the livestream.
        /// </summary>
        public string ExternalId { get; set; }

        /// <summary>
        /// Address of the host.
        /// </summary>
        public string HostAddress { get; set; }

        /// <summary>
        /// Streaming application name.
        /// </summary>
        public string AppName { get; set; }

        /// <summary>
        /// Name of the stream.
        /// </summary>
        public string StreamName { get; set; }

        /// <summary>
        /// Port number.
        /// </summary>
        public ushort Port { get; set; }

        /// <summary>
        /// Username to be used for authenticating the connection.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Password to be used for authenticating the connection.
        /// </summary>
        public string Password { get; set; }

    }

}
