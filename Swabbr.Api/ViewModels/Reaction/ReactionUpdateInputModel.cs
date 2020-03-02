using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace Swabbr.Api.ViewModels
{

    /// <summary>
    /// Input model for updating a reaction.
    /// </summary>
    public class ReactionUpdateInputModel
    {

        /// <summary>
        /// Indicates whether this reaction is public or private.
        /// </summary>
        [JsonProperty("isPrivate")]
        [Required]
        public bool IsPrivate { get; set; }

    }

}
