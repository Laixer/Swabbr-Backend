using System.Runtime.Serialization;

namespace Swabbr.Core.Enums
{

    /// <summary>
    /// Represents the processing state of a reaction.
    /// </summary>
    public enum ReactionState
    {

        [EnumMember(Value = "created")]
        Created,

        [EnumMember(Value = "processing")]
        Processing,

        [EnumMember(Value = "finished")]
        Finished,

        [EnumMember(Value = "failed")]
        Failed,

        [EnumMember(Value = "deleted")]
        Deleted

    }

}
