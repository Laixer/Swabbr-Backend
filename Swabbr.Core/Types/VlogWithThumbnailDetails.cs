using Swabbr.Core.Entities;
using System;

namespace Swabbr.Core.Types
{
    /// <summary>
    ///     Wrapper around a vlog and its thumbnail details.
    /// </summary>
    public sealed class VlogWithThumbnailDetails
    {
        /// <summary>
        ///    <see cref="Vlog"/>.
        /// </summary>
        public Vlog Vlog { get; set; }

        /// <summary>
        ///     Thumbnail download uri.
        /// </summary>
        public Uri ThumbnailUri { get; set; }
    }
}
