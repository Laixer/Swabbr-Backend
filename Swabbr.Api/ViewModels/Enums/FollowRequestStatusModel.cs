namespace Swabbr.Api.ViewModels.Enums
{

    /// <summary>
    /// Represents a <see cref="FollowRequestOutputModel"/> status.
    /// </summary>
    public enum FollowRequestStatusModel
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
