using Swabbr.Core.Enums;
using System;

namespace Swabbr.Core.Entities
{
    /// <summary>
    ///     Represents a video reaction to a vlog.
    /// </summary>
    public class Reaction : EntityBase<Guid>
    {
        /// <summary>
        ///     Id of the user by whom this reaction was created.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        ///     Id of the vlog the reaction responds to.
        /// </summary>
        public Guid TargetVlogId { get; set; }

        /// <summary>
        ///     The moment at which the reaction was posted.
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        ///     Indicates whether this reaction is public or private.
        /// </summary>
        public bool IsPrivate { get; set; }

        /// <summary>
        ///     Represents the length in seconds for this reaction.
        /// </summary>
        public uint LengthInSeconds { get; set; }

        /// <summary>
        ///     Indicates the state of this reaction.
        /// </summary>
        public ReactionState ReactionState { get; set; }
    }
}
