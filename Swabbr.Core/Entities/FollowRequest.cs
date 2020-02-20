using Laixer.Utility.Extensions;
using Swabbr.Core.Enums;
using Swabbr.Core.Types;
using System;

namespace Swabbr.Core.Entities
{

    /// <summary>
    /// Represents an active follow request between two users.
    /// </summary>
    public class FollowRequest : EntityBase<FollowRequestId>
    {

        /// <summary>
        /// Current status of the follow request.
        /// </summary>
        public FollowRequestStatus Status { get; set; }
        public string StatusText => Status.GetEnumMemberAttribute();

        /// <summary>
        /// Timestamp of when the request was initiated.
        /// </summary>
        public DateTimeOffset TimeCreated { get; set; }

    }

}
