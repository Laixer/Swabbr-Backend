using Laixer.Utility.Extensions;
using Swabbr.Core.Enums;
using Swabbr.Core.Types;
using System;

namespace Swabbr.Core.Entities
{

    /// <summary>
    /// Represents an active follow request between two users.
    /// TODO Why use <see cref="RequesterId"/> and <see cref="ReceiverId"/>?
    /// </summary>
    public class FollowRequest : EntityBase<FollowRequestId>
    {

        /// <summary>
        /// Constructor to ensure <see cref="FollowRequestId"/> initialization.
        /// </summary>
        public FollowRequest() => Id = new FollowRequestId();

        /// <summary>
        /// Id of the requesting user.
        /// </summary>
        public Guid RequesterId { get => Id.RequesterId; set => Id.RequesterId = value; }

        /// <summary>
        /// Id of the receiving user.
        /// </summary>
        public Guid ReceiverId { get => Id.ReceiverId; set => Id.ReceiverId = value; }

        /// <summary>
        /// Current status of the follow request.
        /// </summary>
        public FollowRequestStatus FollowRequestStatus { get; set; }

        /// <summary>
        /// Timestamp of when the request was initiated.
        /// </summary>
        public DateTimeOffset TimeCreated { get; set; }

    }

}
