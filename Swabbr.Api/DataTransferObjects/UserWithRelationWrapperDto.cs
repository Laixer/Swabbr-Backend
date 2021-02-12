using Swabbr.Core.Types;
using System;

namespace Swabbr.Api.DataTransferObjects
{
    /// <summary>
    ///     Data class around a user with a <see cref="FollowRequestStatus"/>
    ///     which displays the relation between <see cref="RequestingUserId"/>
    ///     and the <see cref="User"/> object.
    /// </summary>
    public record UserWithRelationWrapperDto
    {
        /// <summary>
        ///     Id of the user that was used to determine
        ///     the <see cref="FollowRequestStatus"/>.
        /// </summary>
        public Guid RequestingUserId { get; set; }

        /// <summary>
        ///     Status of the follow request if one exists between
        ///     <see cref="VlogOwnerId"/> and <see cref="VlogLikingUser"/>,
        ///     else null.
        /// </summary>
        public FollowRequestStatus? FollowRequestStatus { get; set; }

        /// <summary>
        ///     User that liked the vlog.
        /// </summary>
        public UserDto User { get; set; }
    }
}
