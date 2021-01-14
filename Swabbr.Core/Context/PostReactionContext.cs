using System;

namespace Swabbr.Core.Context
{
    /// <summary>
    ///     Context for posting a reaction.
    /// </summary>
    public class PostReactionContext
    {
        /// <summary>
        ///     The user that posts the reaction.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        ///     The suggestive id of the reaction.
        /// </summary>
        public Guid ReactionId { get; set; }

        /// <summary>
        ///     Id of the vlog this reaction will be posted to.
        /// </summary>
        public Guid TargetVlogId { get; set; }

        /// <summary>
        ///     Indicates whether the reaction is public or private.
        /// </summary>
        public bool IsPrivate { get; set; }
    }
}
