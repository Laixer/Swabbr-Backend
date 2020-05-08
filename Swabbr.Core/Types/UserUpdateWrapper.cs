using Swabbr.Core.Enums;
using System;

namespace Swabbr.Core.Types
{

    /// <summary>
    /// Contains all (mostly nullable) properties for updating a <see cref="Entities.SwabbrUser"/>.
    /// </summary>
    public sealed class UserUpdateWrapper
    {

        /// <summary>
        /// Represents the <see cref="Entities.SwabbrUser"/> internal id.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// First name of the user.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Surname of the user.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Selected gender of the user.
        /// </summary>
        public Gender? Gender { get; set; }

        /// <summary>
        /// Selected country.
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Date of birth for the given user.
        /// </summary>
        public DateTime? BirthDate { get; set; }

        /// <summary>
        /// Nickname to display for the user.
        /// </summary>
        public string Nickname { get; set; }

        /// <summary>
        /// Base64 encoded string containing the uploaded profile image of the user.
        /// </summary>
        public string ProfileImageBase64Encoded { get; set; }

        /// <summary>
        /// Indicates whether the profile of the user is publicly visible to other users.
        /// </summary>
        public bool? IsPrivate { get; set; }

    }

}
