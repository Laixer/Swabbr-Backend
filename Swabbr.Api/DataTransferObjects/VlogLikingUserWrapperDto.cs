using System;

namespace Swabbr.Api.DataTransferObjects
{
    /// <summary>
    ///     Data class around a user that likes a certain vlog.
    /// </summary>
    public record VlogLikingUserWrapperDto
    {
        /// <summary>
        ///     User id of the owner of the liked vlog.
        /// </summary>
        public Guid VlogOwnerId { get; set; }

        /// <summary>
        ///     Indicates if <see cref="VlogOwnerId"/> is 
        ///     following <see cref="VlogLikingUser"/>.
        /// </summary>
        public bool IsVlogOwnerFollowingVlogLikingUser { get; set; }

        /// <summary>
        ///     Corresponding vlog like entity.
        /// </summary>
        public VlogLikeDto VlogLike { get; set; }

        /// <summary>
        ///     User that liked the vlog.
        /// </summary>
        public UserDto VlogLikingUser { get; set; }
    }
}
