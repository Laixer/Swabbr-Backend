namespace Swabbr.Core.Types
{
    /// <summary>
    ///     Enum representing a follow request status.
    /// </summary>
    public enum FollowRequestStatus
    {
        /// <summary>
        ///     The follow request is waiting for approval.
        /// </summary>
        Pending = 0,

        /// <summary>
        ///     The follow request has been accepted.
        /// </summary>
        Accepted = 1,

        /// <summary>
        /// The follow request has been declined.
        /// </summary>
        Declined = 2,

        /// <summary>
        ///     There exists no follow request between the states users.
        ///     Note that this value is not stored in the databse, since
        ///     in this case the entity will simply not exist.
        /// </summary>
        NonExistent = 3,
    }
}
