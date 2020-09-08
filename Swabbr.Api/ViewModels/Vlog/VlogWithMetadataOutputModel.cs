using Swabbr.Api.ViewModels.VlogLike;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Swabbr.Api.ViewModels.Vlog
{
    /// <summary>
    ///     Wrapper around a vlog and its thumbnail details.
    /// </summary>
    public sealed class VlogWithMetadataOutputModel
    {
        /// <summary>
        ///     <see cref="VlogOutputModel"/>.
        /// </summary>
        public VlogOutputModel Vlog { get; set; }

        /// <summary>
        ///     Information about the vlog likes.
        /// </summary>
        public VlogLikeSummaryOutputModel VlogLikeSummary { get; set; }

        /// <summary>
        ///     Thumbnail download uri.
        /// </summary>
        public Uri ThumbnailUri { get; set; }
    }
}
