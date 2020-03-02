using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace Swabbr.Api.ViewModels.VlogLike
{

    /// <summary>
    /// Input model for liking a vlog.
    /// </summary>
    public class VlogLikeInputModel
    {

        /// <summary>
        /// Id of the vlog to send the like to.
        /// </summary>
        [Required]
        [JsonProperty("vlogId")]
        public Guid VlogId { get; set; }

        /// <summary>
        /// Id of the user that sent the like.
        /// </summary>
        [Required]
        [JsonProperty("userId")]
        public Guid UserId { get; set; }

    }

}
