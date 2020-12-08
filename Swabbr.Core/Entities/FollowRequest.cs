using Swabbr.Core.Types;
using System;

namespace Swabbr.Core.Entities
{
    /// <summary>
    ///     Represents an active follow request between two users.
    /// </summary>
    public class FollowRequest : EntityBase<FollowRequestId>
    {
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
