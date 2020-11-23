using System.Runtime.Serialization;

namespace Swabbr.Core.Enums
{
    /// <summary>
    ///     Represents the processing state of a reaction.
    /// </summary>
    public enum ReactionState
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
