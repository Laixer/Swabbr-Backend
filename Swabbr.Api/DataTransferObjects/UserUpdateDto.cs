using Swabbr.Core.DataAnnotations;
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
    public record UserUpdateDto
    { 
        /// <summary>
        ///     First name of the user.
        /// </summary>
        [NonEmptyString]
        public string FirstName { get; init; }

        /// <summary>
        ///     Last name of the user.
        /// </summary>
        [NonEmptyString]
        public string LastName { get; init; }

        /// <summary>
        ///     Selected gender of the user.
        /// </summary>
        public Gender? Gender { get; init; }

        // FUTURE Standardization attribute
        /// <summary>
        ///     Selected country.
        /// </summary>
        [StringLength(3, MinimumLength = 3)]
        public string Country { get; init; }

        /// <summary>
        ///     Date of birth for the given user.
        /// </summary>
        public DateTime? BirthDate { get; init; }

        /// <summary>
        ///     The specified time zone of the user.
        /// </summary>
        public string TimeZone { get; init; }

        /// <summary>
        ///     Nickname to display for the user.
        /// </summary>
        [NonEmptyString]
        public string Nickname { get; init; }

        /// <summary>
        ///     Indicates if we have a profile image. Set this to true 
        ///     if an image was uploaded, to false for image deletion.
        /// </summary>
        public bool? HasProfileImage { get; init; }

        /// <summary>
        ///     Angular longitude coordinate.
        /// </summary>
        public double? Longitude { get; init; }

        /// <summary>
        ///     Angular latitude coordinate.
        /// </summary>
        public double? Latitude { get; init; }

        /// <summary>
        ///     Indicates whether the profile of the user is 
        ///     publicly visible to other users.
        /// </summary>
        public bool? IsPrivate { get; init; }

        /// <summary>
        ///     The maximum amount of times the user should be reminded 
        ///     to record a vlog through push notifications.
        /// </summary>
        public uint? DailyVlogRequestLimit { get; init; }

        /// <summary>
        ///     Determines how follow requests are processed for the user.
        /// </summary>
        public FollowMode? FollowMode { get; init; }

        /// <summary>
        ///     Interests.
        /// </summary>
        public string Interest1 { get; set; }
        public string Interest2 { get; set; }
        public string Interest3 { get; set; }
    }
}
