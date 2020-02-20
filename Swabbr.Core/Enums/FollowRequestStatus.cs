using System.Runtime.Serialization;

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
        [EnumMember(Value = "pending")]
        Pending,

        /// <summary>
        /// The follow request has been accepted.
        /// </summary>
        [EnumMember(Value = "accepted")]
        Accepted,

        /// <summary>
        /// The follow request has been declined.
        /// </summary>
        [EnumMember(Value = "declined")]
        Declined

    }

}
