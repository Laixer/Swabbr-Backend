namespace Swabbr.Core.Enums
{
    /// <summary>
    /// Enum that represents the status of an active <see cref="FollowRequest"/> between two users.
    /// </summary>
    public enum FollowRequestStatus
    {
        /// <summary>
        /// The follow request is waiting for approval.
        /// </summary>
        Pending,

        /// <summary>
        /// The follow request has been accepted.
        /// </summary>
        Accepted,

        /// <summary>
        /// The follow request has been declined.
        /// </summary>
        Declined
    }
}