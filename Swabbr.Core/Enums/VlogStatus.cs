namespace Swabbr.Core.Enums
{
    /// <summary>
    ///     Indicates the state of a vlog.
    /// </summary>
    public enum VlogStatus
    {
        /// <summary>
        ///     The vlog is ready to be watched.
        /// </summary>
        UpToDate = 0,

        /// <summary>
        ///     The vlog has been soft deleted.
        /// </summary>
        Deleted = 1
    }
}
