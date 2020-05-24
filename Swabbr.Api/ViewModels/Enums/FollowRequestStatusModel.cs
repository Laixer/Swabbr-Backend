using System.Runtime.Serialization;

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
        Declined,

        /// <summary>
        /// The follow request relation does not exist in the data store.
        /// </summary>
        [EnumMember(Value = "does_not_exist")]
        DoesNotExist

    }
}
