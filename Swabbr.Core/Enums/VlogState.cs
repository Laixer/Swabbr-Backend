using System.Runtime.Serialization;

namespace Swabbr.Core.Enums
{

    /// <summary>
    /// Indicates the state of a request.
    /// </summary>
    public enum VlogState
    {

        [EnumMember(Value = "created")]
        Created,

        [EnumMember(Value = "up_to_date")]
        UpToDate,

        [EnumMember(Value = "deleted")]
        Deleted

    }

}
