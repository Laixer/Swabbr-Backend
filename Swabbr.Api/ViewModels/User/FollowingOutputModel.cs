using System.Collections.Generic;

namespace Swabbr.Api.ViewModels.User
{

    /// <summary>
    /// Viewmodel for displaying a collection of <see cref="UserOutputModel"/>s.
    /// </summary>
    public sealed class FollowingOutputModel
    {

        /// <summary>
        /// Collection of <see cref="UserOutputModel"/>s.
        /// </summary>
        public IEnumerable<UserOutputModel> Following { get; set; }

    }

}
