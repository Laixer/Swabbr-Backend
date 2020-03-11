using System.Runtime.Serialization;

namespace Swabbr.WowzaStreamingCloud.Enums
{

    /// <summary>
    /// Represents a Wowza stream format for outputs.
    /// </summary>
    public enum WscStreamFormat
    {

        [EnumMember(Value = "audioonly")]
        AudioOnly,

        [EnumMember(Value = "videoonly")]
        VideoOnly,

        [EnumMember(Value = "audiovideo")]
        AudioVideo

    }

}
