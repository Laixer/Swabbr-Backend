namespace Swabbr.Api.ViewModels
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
        public UserOutputModel User { get; set; }

    }

}
