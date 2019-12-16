using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Swabbr.Core.Enums
{
    /// <summary>
    /// Enum for the gender of a <see cref="UserItem"/>.
    /// </summary>
    public enum Gender
    {
        /// <summary>
        /// Female gender.
        /// </summary>
        Female = 0,

        /// <summary>
        /// Male gender.
        /// </summary>
        Male = 1,

        /// <summary>
        /// Other or unknown gender.
        /// </summary>
        Unspecified = 2
    }
}