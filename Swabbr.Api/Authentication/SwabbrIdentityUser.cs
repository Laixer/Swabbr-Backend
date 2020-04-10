using Laixer.Utility.Extensions;
using Microsoft.AspNetCore.Identity;
using Swabbr.Core.Entities;
using Swabbr.Core.Enums;
using System;

namespace Swabbr.Api.Authentication
{

    /// <summary>
    /// Custom Identity framework user.
    /// </summary>
    public class SwabbrIdentityUser : IdentityUser<Guid>
    {

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
        public Gender Gender { get; set; }
        public string GenderText => Gender.GetEnumMemberAttribute();

        /// <summary>
        /// Selected country.
        /// TODO Enum or something?
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Date of birth for the given user.
        /// </summary>
        public DateTime BirthDate { get; set; }

        /// <summary>
        /// The specified timezone of the user.
        /// </summary>
        public string Timezone { get; set; }

        /// <summary>
        /// Nickname to display for the user.
        /// </summary>
        public string Nickname { get; set; }

        /// <summary>
        /// URL containing the uploaded profile image of the user.
        /// </summary>
        public Uri ProfileImageUrl { get; set; }

        /// <summary>
        /// Angular longitude coordinate.
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// Angular latitude coordinate.
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// Indicates whether the profile of the user is publicly visible to other users.
        /// </summary>
        public bool IsPrivate { get; set; }

        /// <summary>
        /// The maximum amount of times the user should be reminded to record a vlog through push notifications.
        /// </summary>
        public uint DailyVlogRequestLimit { get; set; }

        /// <summary>
        /// Determines how follow requests are processed for the user.
        /// </summary>
        public FollowMode FollowMode { get; set; }

    }

}
