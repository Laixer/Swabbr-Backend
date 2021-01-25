using Swabbr.Core.Types;
using System;
using System.ComponentModel.DataAnnotations;

namespace Swabbr.Api.DataTransferObjects
{
    /// <summary>
    ///     DTO for a reaction.
    /// </summary>
    public record ReactionDto
    {
        /// <summary>
        ///     Unique reaction identifier.
        /// </summary>
        [Required]
        public Guid Id { get; init; }

        /// <summary>
        ///     Id of the user by whom this reaction was created.
        /// </summary>
        public Guid UserId { get; init; }

        /// <summary>
        ///     Id of the vlog the reaction responds to.
        /// </summary>
        [Required]
        public Guid TargetVlogId { get; init; }

        /// <summary>
        ///     The moment at which the reaction was posted.
        /// </summary>
        public DateTime DateCreated { get; init; }

        /// <summary>
        ///     Indicates whether this reaction is public or private.
        /// </summary>
        public bool IsPrivate { get; init; }

        /// <summary>
        ///     Represents the length in seconds for this reaction.
        /// </summary>
        public uint? Length { get; init; }

        /// <summary>
        ///     Indicates the state of this reaction.
        /// </summary>
        public ReactionStatus ReactionStatus { get; init; }

        /// <summary>
        ///     Reaction video download uri.
        /// </summary>
        public Uri VideoUri { get; init; }

        /// <summary>
        ///     Thumbnail download uri.
        /// </summary>
        public Uri ThumbnailUri { get; init; }
    }
}
