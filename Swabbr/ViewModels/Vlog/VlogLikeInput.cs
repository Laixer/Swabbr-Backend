using Newtonsoft.Json;

namespace Swabbr.Api.ViewModels
{
    public class VlogLikeInput
    {
        /// <summary>
        /// Id of the vlog to send the like to.
        /// </summary>
        [JsonProperty("vlogId")]
        public string VlogId { get; set; }

        /// <summary>
        /// Id of the user that sent the like.
        /// </summary>
        [JsonProperty("userId")]
        public string UserId { get; set; }
    }
}