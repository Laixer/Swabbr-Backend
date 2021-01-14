using Swabbr.Core.Types;
using System;

namespace Swabbr.Api.DataTransferObjects
{
    /// <summary>
    ///     DTO for a follow request.
    /// </summary>
    public record FollowRequestDto
    {
        /// <summary>
        ///     Id of the requesting user.
        /// </summary>
        public Guid RequesterId { get; init; }

        /// <summary>
        ///     Id of the receiving user.
        /// </summary>
        public Guid ReceiverId { get; init; }

        /// <summary>
        ///     Current status of the follow request.
        /// </summary>
        public FollowRequestStatus FollowRequestStatus { get; init; }

        /// <summary>
        ///     Timestamp of when the request was initiated.
        /// </summary>
        public DateTimeOffset DateCreated { get; init; }

        /// <summary>
        ///     Timestamp of when the request was updated.
        /// </summary>
        public DateTimeOffset? DateUpdated { get; init; }
    }
}
