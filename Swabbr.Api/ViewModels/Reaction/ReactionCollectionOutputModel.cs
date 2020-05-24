using System.Collections.Generic;
using System.Linq;

namespace Swabbr.Api.ViewModels.Reaction
{

    /// <summary>
    /// Contains a collection of <see cref="ReactionOutputModel"/>s.
    /// </summary>
    public sealed class ReactionCollectionOutputModel
    {

        public int ReactionCount => Reactions.Count();

        public IEnumerable<ReactionOutputModel> Reactions { get; set; }

    }

}
