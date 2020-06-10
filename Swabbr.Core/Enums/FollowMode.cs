using NpgsqlTypes;
using System.Runtime.Serialization;

namespace Swabbr.Core.Enums
{

    /// <summary>
    /// Enum representation of the follow mode setting for a profile.
    /// </summary>
    public enum FollowMode
    {

        /// <summary>
        /// Manually accept or deny incoming follow requests.
        /// </summary>
        [EnumMember(Value = "manual")]
        Manual,

        /// <summary>
        /// Automatically accept all incoming follow requests.
        /// </summary>
        [EnumMember(Value = "accept_all")]
        AcceptAll,

        /// <summary>
        /// Automatically deny all incoming follow requests.
        /// </summary>
        [EnumMember(Value = "decline_all")]
        DeclineAll

    }

}
