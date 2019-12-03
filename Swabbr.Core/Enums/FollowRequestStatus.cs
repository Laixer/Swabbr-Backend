namespace Swabbr.Core.Enums
{
    /// <summary>
    /// Enum that represents the status of an active <see cref="FollowRequest" /> between two users.
    /// </summary>
    public enum FollowRequestStatus
    {
        /// <summary>
        /// The follow request is waiting for approval.
        /// </summary>
        Pending = 0,

        /// <summary>
        /// The follow request has been accepted.
        /// </summary>
        Accepted = 1,

        /// <summary>
        /// The follow request has been declined.
        /// </summary>
        Declined = 2
    }
}