using System.Runtime.Serialization;

namespace Swabbr.Core.Enums
{
    /// <summary>
    ///     Indicates the state of a request.
    /// </summary>
    public enum VlogState
    {
        /// <summary>
        ///     The vlog is ready to be watched.
        /// </summary>
        [EnumMember(Value = "up_to_date")]
        UpToDate,

        /// <summary>
        ///     The vlog has been soft deleted.
        /// </summary>
        [EnumMember(Value = "deleted")]
        Deleted
    }
}
