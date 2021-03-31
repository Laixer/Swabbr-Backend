using Swabbr.Core.Types;
using System;

namespace Swabbr.Api.DataTransferObjects
{
    /// <summary>
    ///     DTO for a user.
    /// </summary>
    /// <remarks>
    ///     This contains everything about a user, including
    ///     private information. For the public variant of 
    ///     this DTO, <see cref="UserDto"/>.
    /// </remarks>
    public record UserCompleteDto
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
        public string Nickname { get; init; }

        /// <summary>
        ///     Indicates if we have a profile image.
        /// </summary>
        public bool HasProfileImage { get; init; }

        /// <summary>
        ///     Profile image download uri. Null if we have none.
        /// </summary>
        public Uri ProfileImageUri { get; set; }

        /// <summary>
        ///     Can be used to update the profile image.
        /// </summary>
        public Uri ProfileImageUploadUri { get; set; }

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
        public bool IsPrivate { get; init; }

        /// <summary>
        ///     The maximum amount of times the user should be reminded 
        ///     to record a vlog through push notifications.
        /// </summary>
        public uint DailyVlogRequestLimit { get; init; }

        /// <summary>
        ///     Determines how follow requests are processed for the user.
        /// </summary>
        public FollowMode FollowMode { get; init; }
    }
}
