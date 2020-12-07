using Swabbr.Core.Enums;
using System;

namespace Swabbr.Api.DataTransferObjects
{
    /// <summary>
    ///     DTO for a follow request.
    /// </summary>
    public class FollowRequestDto
    {
        /// <summary>
        ///     Id of the requesting user.
        /// </summary>
        public Guid RequesterId { get; set; }

        /// <summary>
        ///     Id of the receiving user.
        /// </summary>
        public Guid ReceiverId { get; set; }

        /// <summary>
        ///     Current status of the follow request.
        /// </summary>
        public FollowRequestStatus FollowRequestStatus { get; set; }

        /// <summary>
        ///     Timestamp of when the request was initiated.
        /// </summary>
        public DateTimeOffset DateCreated { get; set; }

        /// <summary>
        ///     Timestamp of when the request was updated.
        /// </summary>
        public DateTimeOffset? DateUpdated { get; set; }
    }
}
