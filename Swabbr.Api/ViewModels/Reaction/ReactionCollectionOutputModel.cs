using System.Collections.Generic;
using System.Linq;

namespace Swabbr.Api.ViewModels.Reaction
{
    /// <summary>
    /// Contains a collection of <see cref="ReactionOutputModel"/>s.
    /// </summary>
    public sealed class ReactionCollectionOutputModel
    {
        /// <summary>
        ///     Represents the total amount of reactions for a vlog.
        /// </summary>
        public uint ReactionTotalCount { get; set; }

        /// <summary>
        ///     Represents the amount of reactions in the 
        ///     <see cref="Reactions"/> field.
        /// </summary>
        public uint ReactionCount => (uint) Reactions.Count();

        /// <summary>
        ///     Some or all reactions.
        /// </summary>
        public IEnumerable<ReactionWrapperOutputModel> Reactions { get; set; }
    }
}
