using System.Runtime.Serialization;

namespace Swabbr.Api.ViewModels.Enums
{

    /// <summary>
    /// Represents a follow mode.
    /// </summary>
    public enum FollowModeModel
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
