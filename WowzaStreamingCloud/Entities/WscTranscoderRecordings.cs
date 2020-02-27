using Newtonsoft.Json;

namespace Swabbr.WowzaStreamingCloud.Entities
{

    /// <summary>
    /// Response object for the transcoders/id/recordings wowza api endpoint.
    /// </summary>
    public sealed class WscTranscoderRecordings
    {

        [JsonProperty("recordings")]
        public SubTranscoderRecordings[] Recordings { get; set; }

    }

    /// <summary>
    /// Subclass of <see cref="WscTranscoderRecordings"/>.
    /// </summary>
    public sealed class SubTranscoderRecordings
    {

        [JsonProperty("id")]
        public string Id { get; set; }

    }

}

