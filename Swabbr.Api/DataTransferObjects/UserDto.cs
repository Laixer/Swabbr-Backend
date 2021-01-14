﻿using Swabbr.Core.Types;
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
    public record UserDto
    {
        /// <summary>
        ///     Unique user identifier.
        /// </summary>
        public Guid Id { get; init; }

        /// <summary>
        ///     First name of the user.
        /// </summary>
        public string FirstName { get; init; }

        /// <summary>
        ///     Last name of the user.
        /// </summary>
        public string LastName { get; init; }

        /// <summary>
        ///     Selected gender of the user.
        /// </summary>
        public Gender? Gender { get; init; }

        /// <summary>
        ///     Selected country.
        /// </summary>
        public string Country { get; init; }

        /// <summary>
        ///     Nickname to display for the user.
        /// </summary>
        public string Nickname { get; init; }

        /// <summary>
        ///     Base64 encoded string containing the uploaded 
        ///     profile image of the user.
        /// </summary>
        public string ProfileImageBase64Encoded { get; init; }
    }
}
