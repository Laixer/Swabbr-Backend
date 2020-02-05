using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Swabbr.Api.ViewModels
{
    public class VlogUpdateModel
    {
        /// <summary>
        /// A collection of user id's to share the vlog with.
        /// </summary>
        [JsonProperty("sharedUsers")]
        public List<Guid> SharedUsers { get; set; }

        /// <summary>
        /// Indicates if the vlog should be publicly available to other users.
        /// </summary>
        [JsonProperty("isPrivate")]
        public bool IsPrivate { get; set; }
    }
}