using Newtonsoft.Json;
using System;

namespace Swabbr.Api.ViewModels
{

    /// <summary>
    /// Represents a single reaction.
    /// </summary>
    public class ReactionOutputModel
    {

        /// <summary>
        /// Id of the reaction.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Id of the user by whom this reaction was created.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Id of the vlog the reaction responds to.
        /// </summary>
        public Guid TargetVlogId { get; set; }

        /// <summary>
        /// The moment at which the reaction was posted.
        /// </summary>
        public DateTimeOffset CreateDate { get; set; }

        /// <summary>
        /// Indicates whether this reaction is public or private.
        /// </summary>
        public bool IsPrivate { get; set; }

    }

}
