using System.Runtime.Serialization;

namespace Swabbr.Core.Enums
{
    /// <summary>
    ///     Represents the processing state of a reaction.
    /// </summary>
    public enum ReactionState
    {
        /// <summary>
        ///     Reaction has been created without content.
        /// </summary>
        [EnumMember(Value = "created")]
        Created,

        /// <summary>
        ///     Reaction content is being processed.
        /// </summary>
        [EnumMember(Value = "processing")]
        Processing,

        /// <summary>
        ///     Transcoding the reaction finished. 
        ///     This reaction is ready for playback.
        /// </summary>
        [EnumMember(Value = "finished")]
        Finished,

        /// <summary>
        ///     Transcoding the reaction failed.
        /// </summary>
        [EnumMember(Value = "failed")]
        Failed,

        /// <summary>
        ///     The reaction has been soft deleted.
        /// </summary>
        [EnumMember(Value = "deleted")]
        Deleted
    }
}
