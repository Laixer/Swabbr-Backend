using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace Swabbr.Api.ViewModels
{

    /// <summary>
    /// Input model for placing a reaction.
    /// </summary>
    public class ReactionInputModel
    {

        /// <summary>
        /// Id of the <see cref="Core.Entities.Vlog"/> the reaction responds to.
        /// </summary>
        [Required]
        public Guid TargetVlogId { get; set; }

        /// <summary>
        /// Indicates whether this reaction is public or private.
        /// </summary>
        public bool IsPrivate { get; set; }

    }

}
