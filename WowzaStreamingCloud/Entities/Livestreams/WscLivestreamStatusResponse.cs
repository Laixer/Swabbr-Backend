using Newtonsoft.Json;
using Swabbr.WowzaStreamingCloud.Enums;

namespace Swabbr.WowzaStreamingCloud.Entities.Livestreams
{

    /// <summary>
    /// Response object we get when polling a livestream status.
    /// https://api.cloud.wowza.com/api/v1.4/live_streams/state
    /// </summary>
    internal sealed class WscLivestreamStatusResponse
    {

        /// <summary>
        /// Nested livestream info object.
        /// </summary>
        [JsonProperty("live_stream")]
        public WscLivestreamStateInfo LiveStream { get; set; }

    }

    internal sealed class WscLivestreamStateInfo
    {

        /// <summary>
        /// Displays the current state of the livestream.
        /// </summary>
        [JsonProperty("state")]
        public WscLivestreamState State { get; set; }

        /// <summary>
        /// Displays the ip address at which the livestream can be found.
        /// </summary>
        [JsonProperty("ip_address")]
        public string IpAddress { get; set; }

    }

}
