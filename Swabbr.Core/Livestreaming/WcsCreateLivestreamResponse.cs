namespace Swabbr.Core.Livestreaming
{
    using Newtonsoft.Json;

    /// <summary>
    /// Represents a JSON object response for a new Wowza Streaming Cloud livestream.
    /// </summary>
    public partial class WcsCreateLivestreamResponse
    {
        [JsonProperty("live_stream")]
        public WcsLivestream Livestream { get; set; }
    }
}
