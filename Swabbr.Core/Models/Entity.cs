using Newtonsoft.Json;

namespace Swabbr.Core.Models
{
    public abstract class Entity
    {
        /// <summary>
        /// Entity identifier
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }
    }
}
