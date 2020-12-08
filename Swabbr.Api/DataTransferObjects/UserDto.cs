using Swabbr.Core.Types;
using System;

namespace Swabbr.Api.DataTransferObjects
{
    /// <summary>
    ///     DTO for a user.
    /// </summary>
    /// <remarks>
    ///     This only contains information which can be
    ///     considered to be public. For settings and
    ///     personal details, <see cref="UserDto"/>
    /// </remarks>
    public class UserDto
    {
        /// <summary>
        ///     Unique user identifier.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        ///     First name of the user.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        ///     Last name of the user.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        ///     Selected gender of the user.
        /// </summary>
        public Gender? Gender { get; set; }

        /// <summary>
        ///     Selected country.
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        ///     Nickname to display for the user.
        /// </summary>
        public string Nickname { get; set; }

        /// <summary>
        ///     Base64 encoded string containing the uploaded 
        ///     profile image of the user.
        /// </summary>
        public string ProfileImageBase64Encoded { get; set; }
    }
}
