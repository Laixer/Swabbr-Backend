using Newtonsoft.Json;

namespace Swabbr.Api.ViewModels
{
    public class VlogUpdateModel
    {
        /// <summary>
        /// Indicates if the vlog should be publicly available to other users.
        /// </summary>
        [JsonProperty("isPrivate")]
        public bool IsPrivate { get; set; }
    }
}