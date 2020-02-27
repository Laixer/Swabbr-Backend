using System.Runtime.Serialization;

namespace Swabbr.WowzaStreamingCloud.Enums
{

    /// <summary>
    /// Represents the state of a Wowza livestream.
    /// </summary>
    internal enum WscLivestreamState
    {

        [EnumMember(Value = "starting")]
        Starting,

        [EnumMember(Value = "started")]
        Started,

        [EnumMember(Value = "stopping")]
        Stopping,

        [EnumMember(Value = "stopped")]
        Stopped,

        [EnumMember(Value = "resetting")]
        Resetting

    }

}
