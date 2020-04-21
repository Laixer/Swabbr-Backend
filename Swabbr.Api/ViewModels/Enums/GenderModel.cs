using System.Runtime.Serialization;

namespace Swabbr.Api.ViewModels.Enums
{

    /// <summary>
    /// Represents a gender.
    /// </summary>
    public enum GenderModel
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
