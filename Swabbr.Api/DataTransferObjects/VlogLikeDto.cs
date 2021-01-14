using System;

namespace Swabbr.Api.DataTransferObjects
{
    /// <summary>
    ///     DTO for a vlog like.
    /// </summary>
    public record VlogLikeDto
    {
        /// <summary>
        ///     The internal vlog id.
        /// </summary>
        public Guid VlogId { get; init; }

        /// <summary>
        ///     The internal user id.
        /// </summary>
        public Guid UserId { get; init; }

        /// <summary>
        ///     The time at which the user liked the vlog.
        /// </summary>
        public DateTimeOffset DateCreated { get; init; }
    }
}
