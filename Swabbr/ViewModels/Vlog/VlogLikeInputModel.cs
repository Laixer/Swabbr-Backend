using Newtonsoft.Json;
using Swabbr.Core.Entities;
using System;
using System.ComponentModel.DataAnnotations;

namespace Swabbr.Api.ViewModels
{
    public class VlogLikeInputModel
    {
        /// <summary>
        /// Id of the vlog to send the like to.
        /// </summary>
        [Required, JsonProperty("vlogId")]
        public Guid VlogId { get; set; }

        /// <summary>
        /// Id of the user that sent the like.
        /// </summary>
        [Required, JsonProperty("userId")]
        public Guid UserId { get; set; }

        public static implicit operator VlogLike(VlogLikeInputModel entity)
            => new VlogLike
            {
                UserId = entity.UserId,
                VlogId = entity.VlogId
            };
    }
}