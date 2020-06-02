using System.Runtime.Serialization;

namespace Swabbr.Core.Enums
{

    /// <summary>
    /// Enum for the gender of a <see cref="Core.Entities.SwabbrUser"/>.
    /// </summary>
    public enum Gender
    {

        /// <summary>
        /// Female gender.
        /// </summary>
        [EnumMember(Value = "female")]
        Female,

        /// <summary>
        /// Male gender.
        /// </summary>
        [EnumMember(Value = "male")]
        Male,

        /// <summary>
        /// Other or unknown gender.
        /// </summary>
        [EnumMember(Value = "unspecified")]
        Unspecified

    }

}
