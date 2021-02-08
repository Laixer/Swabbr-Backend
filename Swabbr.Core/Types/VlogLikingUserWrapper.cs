using Swabbr.Core.Entities;
using System;

namespace Swabbr.Core.Types
{
    /// <summary>
    ///     Data class around a user that likes a certain vlog.
    /// </summary>
    public record VlogLikingUserWrapper
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
        public VlogLike VlogLike { get; set; }

        /// <summary>
        ///     User that liked the vlog.
        /// </summary>
        public User VlogLikingUser { get; set; }
    }
}
