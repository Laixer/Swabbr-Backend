using Swabbr.Core.Enums;
using Swabbr.Core.Types;
using System;

namespace Swabbr.Core.Entities
{
    /// <summary>
    ///     Represents an active follow request between two users.
    /// </summary>
    public class FollowRequest : EntityBase<FollowRequestId>
    {
        // TODO Check this with Dapper.
        /// <summary>
        ///     Constructor to ensure <see cref="FollowRequestId"/> initialization.
        /// </summary>
        public FollowRequest() => Id = new FollowRequestId();

        /// <summary>
        ///     Current status of the follow request.
        /// </summary>
        public FollowRequestStatus FollowRequestStatus { get; set; }

        /// <summary>
        ///     Timestamp of when the request was initiated.
        /// </summary>
        public DateTimeOffset TimeCreated { get; set; }
    }
}
