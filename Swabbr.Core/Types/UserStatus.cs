namespace Swabbr.Core.Types
{
    /// <summary>
    ///     Enum representing a user status.
    /// </summary>
    public enum UserStatus
    {
        /// <summary>
        ///     The user is public.
        /// </summary>
        UpToDate = 0,

        /// <summary>
        ///     The user has been soft deleted.
        /// </summary>
        Deleted = 1
    }
}
