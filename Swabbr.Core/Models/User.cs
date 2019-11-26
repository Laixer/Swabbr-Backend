using Swabbr.Core.Enums;
using System;

namespace Swabbr.Core.Models
{
    /// <summary>
    /// Represents a single user.
    /// </summary>
    public class User : Entity
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

        /// <summary>
        /// Selected country.
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Email address.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Hashed password of the user.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Date of birth for the given user.
        /// </summary>
        public DateTime BirthDate { get; set; }

        /// <summary>
        /// The specified timezone of the user
        /// </summary>
        public string Timezone { get; set; }

        /// <summary>
        /// Nickname to display for the user.
        /// </summary>
        public string Nickname { get; set; }

        /// <summary>
        /// URL containing the uploaded profile image of the user.
        /// </summary>
        public string ProfileImageUrl { get; set; }

        /// <summary>
        /// Indicates whether the profile of the user is publicly visible to other users.
        /// </summary>
        public bool IsPrivate { get; set; }

        /// <summary>
        /// Angular longitude coordinate.
        /// </summary>
        public decimal Longitude { get; set; }

        /// <summary>
        /// Angular latitude coordinate.
        /// </summary>
        public decimal Latitude { get; set; }

        /// <summary>
        /// Phone number of the user stored as text.
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Personal settings and preferences.
        /// </summary>
        public Settings Settings { get; set; }
    }
}