using Swabbr.Core.Types;
using System;

namespace Swabbr.Api.DataTransferObjects
{
    /// <summary>
    ///     Data class around a user that likes a certain vlog.
    /// </summary>
    public record VlogLikingUserWrapperDto : UserWithRelationWrapperDto
    {
        /// <summary>
        ///     Corresponding vlog like entity.
        /// </summary>
        public VlogLikeDto VlogLike { get; set; }
    }
}
