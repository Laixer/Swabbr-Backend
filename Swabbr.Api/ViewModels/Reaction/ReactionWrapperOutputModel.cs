using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Swabbr.Api.ViewModels.Reaction
{
    /// <summary>
    ///     Wrapper for a reaction and its thumbnail uri.
    /// </summary>
    public class ReactionWrapperOutputModel
    {
        /// <summary>
        ///     <see cref="ReactionOutputModel"/>.
        /// </summary>
        public ReactionOutputModel Reaction { get; set; }

        /// <summary>
        ///     Thumbnail uri.
        /// </summary>
        public Uri ThumbnailUri { get; set; }
    }
}
