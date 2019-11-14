using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace Swabbr.Core.Enums
{
    /// <summary>
    /// Enum representation of the follow mode setting for the profile of a <see cref="UserItem" />.
    /// </summary>
    public enum FollowMode
    {
        /// <summary>
        /// Manually accept or deny incoming follow requests.
        /// </summary>
        Manual = 0,

        /// <summary>
        /// Automatically accept all incoming follow requests.
        /// </summary>
        AcceptAll = 1,

        /// <summary>
        /// Automatically deny all incoming follow requests.
        /// </summary>
        DenyAll = 2
    }
}