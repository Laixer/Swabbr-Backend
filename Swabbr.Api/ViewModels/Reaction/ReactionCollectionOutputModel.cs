using System.Collections.Generic;

namespace Swabbr.Api.ViewModels.Reaction
{

    /// <summary>
    /// Contains a collection of <see cref="ReactionOutputModel"/>s.
    /// </summary>
    public sealed class ReactionCollectionOutputModel
    {

        public IEnumerable<ReactionOutputModel> Reactions { get; set; }

    }

}
