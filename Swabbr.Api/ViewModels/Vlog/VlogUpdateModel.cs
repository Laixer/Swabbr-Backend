using Newtonsoft.Json;
using Swabbr.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Swabbr.Api.ViewModels
{
    public class VlogUpdateModel
    {
        /// <summary>
        /// Id of the vlog.
        /// </summary>
        [JsonProperty("vlogId")]
        public Guid VlogId { get; set; }

        /// <summary>
        /// Indicates if the vlog should be publicly available to other users.
        /// </summary>
        [JsonProperty("isPrivate")]
        public bool IsPrivate { get; set; }
    }
}
