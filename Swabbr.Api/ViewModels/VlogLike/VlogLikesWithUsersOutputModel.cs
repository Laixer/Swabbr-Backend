using Swabbr.Api.ViewModels.User;
using System.Collections.Generic;

namespace Swabbr.Api.ViewModels.VlogLike
{

    /// <summary>
    /// Wrapper for the vlog like count with their users for a given vlog.
    /// </summary>
    public sealed class VlogLikesWithUsersOutputModel
    {

        public int TotalLikeCount { get; set; }

        public IEnumerable<UserSimplifiedOutputModel> UsersSimplified { get; set; }

    }

}
