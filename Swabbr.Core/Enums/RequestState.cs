using System.Runtime.Serialization;

namespace Swabbr.Core.Enums
{

    /// <summary>
    /// Indicates the state of a request.
    /// </summary>
    public enum RequestState
    {

        [EnumMember(Value = "created")]
        Created,

        [EnumMember(Value = "sent")]
        Sent,

        [EnumMember(Value = "accepted")]
        Accepted,

        [EnumMember(Value = "rejected")]
        Rejected,

        [EnumMember(Value = "timed_out")]
        TimedOut

    }

}
