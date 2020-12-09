﻿using Swabbr.Core.DataAnnotations;
using Swabbr.Core.Types;
using System;
using System.ComponentModel.DataAnnotations;

namespace Swabbr.Api.DataTransferObjects
{
    /// <summary>
    ///     DTO for updating a user.
    /// </summary>
    /// <remarks>
    ///     All properties in this DTO are nullable. This should
    ///     be used in conjunction with <see cref="Helpers.UserUpdateHelper"/>
    ///     to ensure only assigned properties are updated.
    /// </remarks>
    public class UserUpdateDto
    { 
        /// <summary>
        ///     First name of the user.
        /// </summary>
        [NonEmptyString]
        public string FirstName { get; set; }

        /// <summary>
        ///     Last name of the user.
        /// </summary>
        [NonEmptyString]
        public string LastName { get; set; }

        /// <summary>
        ///     Selected gender of the user.
        /// </summary>
        public Gender? Gender { get; set; }

        // FUTURE Standardization attribute
        /// <summary>
        ///     Selected country.
        /// </summary>
        [StringLength(3, MinimumLength = 3)]
        public string Country { get; set; }

        /// <summary>
        ///     Date of birth for the given user.
        /// </summary>
        public DateTime? BirthDate { get; set; }

        /// <summary>
        ///     The specified timezone of the user.
        /// </summary>
        public TimeZoneInfo Timezone { get; set; }

        /// <summary>
        ///     Nickname to display for the user.
        /// </summary>
        [NonEmptyString]
        public string Nickname { get; set; }

        /// <summary>
        ///     Base64 encoded string containing the uploaded 
        ///     profile image of the user.
        /// </summary>
        [Base64Encoded]
        public string ProfileImageBase64Encoded { get; set; }

        /// <summary>
        ///     Angular longitude coordinate.
        /// </summary>
        public double? Longitude { get; set; }

        /// <summary>
        ///     Angular latitude coordinate.
        /// </summary>
        public double? Latitude { get; set; }

        /// <summary>
        ///     Indicates whether the profile of the user is 
        ///     publicly visible to other users.
        /// </summary>
        public bool? IsPrivate { get; set; }

        /// <summary>
        ///     The maximum amount of times the user should be reminded 
        ///     to record a vlog through push notifications.
        /// </summary>
        public uint? DailyVlogRequestLimit { get; set; }

        /// <summary>
        ///     Determines how follow requests are processed for the user.
        /// </summary>
        public FollowMode? FollowMode { get; set; }
    }
}