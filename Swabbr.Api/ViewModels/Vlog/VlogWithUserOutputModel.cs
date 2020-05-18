using Swabbr.Api.ViewModels.User;

namespace Swabbr.Api.ViewModels.Vlog
{

    /// <summary>
    /// Contains both a vlog and the corresponding user.
    /// </summary>
    public class VlogWithUserOutputModel
    {

        /// <summary>
        /// The vlog.
        /// </summary>
        public VlogOutputModel Vlog { get; set; }

        /// <summary>
        /// The user that owns the vlog.
        /// </summary>
        public UserWithStatsOutputModel User { get; set; }

    }

}
