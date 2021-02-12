using Swabbr.Core.Entities;

namespace Swabbr.Core.Types
{
    /// <summary>
    ///     Data class around a user that likes a certain vlog.
    /// </summary>
    public record VlogLikingUserWrapper : UserWithRelationWrapper
    {
        /// <summary>
        ///     Corresponding vlog like entity.
        /// </summary>
        public VlogLike VlogLike { get; set; }
    }
}
