using Newtonsoft.Json;
using System;

namespace Swabbr.WowzaStreamingCloud.Entities.Livestreams
{
    /// <summary>
    /// Response for livestream get from Wowza API.
    /// </summary>
    internal sealed class WscLivestreamResponse
    {

        [JsonProperty("live_stream")]
        public SubWscLivestreamResponse Livestream { get; set; }

    }

    /// <summary>
    /// Represents a single livestream response.
    /// </summary>
    internal sealed class SubWscLivestreamResponse
    {

        /// <summary>
        /// Livestream wowza id.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Wrapper for connection info.
        /// </summary>
        [JsonProperty("source_connection_information")]
        public SubWscSourceConnectionInformation SourceConnectionInformation { get; set; }

    }

    /// <summary>
    /// Displays the connection information for a Wowza Livestream.
    /// </summary>
    internal sealed class SubWscSourceConnectionInformation
    {

        /// <summary>
        /// Endpoint to stream to.
        /// </summary>
        [JsonProperty("primary_server")]
        public Uri PrimaryServer { get; set; }

        /// <summary>
        /// Port to stream to.
        /// </summary>
        [JsonProperty("host_port")]
        public int HostPort { get; set; }

        /// <summary>
        /// Application to append to the host port.
        /// </summary>
        [JsonProperty("application")]
        public string Application { get; set; }
        
        /// <summary>
        /// Also used as stream key by other services.
        /// </summary>
        [JsonProperty("stream_name")]
        public string StreamName { get; set; }

        /// <summary>
        /// Upstream authentication username.
        /// </summary>
        [JsonProperty("username")]
        public string Username { get; set; }

        /// <summary>
        /// Upstream authentication password.
        /// </summary>
        [JsonProperty("password")]
        public string Password { get; set; }

    }

}
