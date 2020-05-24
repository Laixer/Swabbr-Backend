using Swabbr.Api.ViewModels.VlogLike;
using System.Collections.Generic;

namespace Swabbr.Api.ViewModels.Vlog
{

    /// <summary>
    /// Wrapper for a <see cref="VlogOutputModel"/> and its corresponding
    /// <see cref="VlogLikeOutputModel"/> objects.
    /// </summary>
    public sealed class VlogWithLikesOutputModel
    {

        public VlogOutputModel Vlog { get; set; }

        public IEnumerable<VlogLikeOutputModel> VlogLikes { get; set; }

    }

}
