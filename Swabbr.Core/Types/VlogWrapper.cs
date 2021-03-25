using Swabbr.Core.Entities;

namespace Swabbr.Core.Types
{
    /// <summary>
    ///     Wrapper around a reaction with a user.
    /// </summary>
    public sealed class VlogWrapper
    {
        /// <summary>
        ///     Vlog.
        /// </summary>
        public Vlog Vlog { get; init; }

        /// <summary>
        ///     Corresponding user.
        /// </summary>
        public User User { get; init; }

        /// <summary>
        ///     Total amount of likes.
        /// </summary>
        public int VlogLikeCount { get; init; }

        /// <summary>
        ///     Total amount of reactions.
        /// </summary>
        public int ReactionCount { get; init; }
    }
}
