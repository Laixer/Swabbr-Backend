using Swabbr.Core.Types;
using System;

namespace Swabbr.Core.Entities
{
    /// <summary>
    ///     Represents a single user.
    /// </summary>
    public class User : EntityBase<Guid>
    {
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

        // FUTURE Standardize with some type or enum
        /// <summary>
        ///     Selected country.
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        ///     Date of birth for the given user.
        /// </summary>
        public DateTime? BirthDate { get; set; }

        /// <summary>
        ///     The specified time zone of the user.
        /// </summary>
        public TimeZoneInfo TimeZone { get; set; }

        /// <summary>
        ///     Nickname to display for the user.
        /// </summary>
        public string Nickname { get; set; }

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
        public bool IsPrivate { get; set; }

        /// <summary>
        ///     Indicates if we have a profile image.
        /// </summary>
        public bool HasProfileImage { get; set; }

        /// <summary>
        ///     Profile image download uri. Null if we have none.
        /// </summary>
        public Uri ProfileImageUri { get; set; }

        /// <summary>
        ///     The maximum amount of times the user should be reminded 
        ///     to record a vlog through push notifications.
        /// </summary>
        public uint DailyVlogRequestLimit { get; set; }

        /// <summary>
        ///     Determines how follow requests are processed for the user.
        /// </summary>
        public FollowMode FollowMode { get; set; }
    }
}
