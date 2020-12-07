using Swabbr.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Swabbr.Api.DataTransferObjects
{
    /// <summary>
    ///     DTO for a reaction.
    /// </summary>
    public class ReactionDto
    {
        /// <summary>
        ///     Unique reaction identifier.
        /// </summary>
        public Guid Id { get; set; }

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
        public DateTime DateCreated { get; set; }

        /// <summary>
        ///     Indicates whether this reaction is public or private.
        /// </summary>
        public bool IsPrivate { get; set; }

        /// <summary>
        ///     Represents the length in seconds for this reaction.
        /// </summary>
        public uint? LengthInSeconds { get; set; }

        /// <summary>
        ///     Indicates the state of this reaction.
        /// </summary>
        public ReactionStatus ReactionStatus { get; set; }
    }
}
