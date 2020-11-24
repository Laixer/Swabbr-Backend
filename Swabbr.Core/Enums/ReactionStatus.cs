using System.Runtime.Serialization;

namespace Swabbr.Core.Enums
{
    /// <summary>
    ///     Represents status of a reaction.
    /// </summary>
    public enum ReactionStatus
    {
        /// <summary>
        ///     The reaction is ready to be watched.
        /// </summary>
        [EnumMember(Value = "up_to_date")]
        UpToDate,

        /// <summary>
        ///     The reaction has been soft deleted.
        /// </summary>
        [EnumMember(Value = "deleted")]
        Deleted
    }
}
