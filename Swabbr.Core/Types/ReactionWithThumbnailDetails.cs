using Swabbr.Core.Entities;
using System;

namespace Swabbr.Core.Types
{
    /// <summary>
    ///     Wrapper for a reaction and its thumbnail details.
    /// </summary>
    public sealed class ReactionWithThumbnailDetails
    {
        /// <summary>
        ///    <see cref="Reaction"/>.
        /// </summary>
        public Reaction Reaction { get; set; }

        /// <summary>
        ///     Thumbnail download uri.
        /// </summary>
        public Uri ThumbnailUri { get; set; }
    }
}
