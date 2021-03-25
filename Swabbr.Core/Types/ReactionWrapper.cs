using Swabbr.Core.Entities;

namespace Swabbr.Core.Types
{
    /// <summary>
    ///     Wrapper around a reaction with a user.
    /// </summary>
    public sealed class ReactionWrapper
    {
        /// <summary>
        ///     Reaction.
        /// </summary>
        public Reaction Reaction { get; init; }

        /// <summary>
        ///     Corresponding user.
        /// </summary>
        public User User { get; init; }
    }
}
