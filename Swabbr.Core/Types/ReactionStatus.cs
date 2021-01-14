namespace Swabbr.Core.Types
{
    /// <summary>
    ///     Represents status of a reaction.
    /// </summary>
    public enum ReactionStatus
    {
        /// <summary>
        ///     The reaction is ready to be watched.
        /// </summary>
        UpToDate = 0,

        /// <summary>
        ///     The reaction has been soft deleted.
        /// </summary>
        Deleted = 1
    }
}
