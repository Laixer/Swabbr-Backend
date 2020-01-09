using Microsoft.Azure.Cosmos.Table;
using System;

namespace Swabbr.Infrastructure.Data.Entities
{
    /// <summary>
    /// Represents a single user.
    /// </summary>
    public class UserTableEntity : TableEntity
    {
        public UserTableEntity()
        {
        }

        /// <summary>
        /// Unique identifier.
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
        /// Email address of the user.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Selected gender of the user represented by an integer.
        /// </summary>
        public int Gender { get; set; }

        /// <summary>
        /// Selected country.
        /// </summary>
        public string Country { get; set; }

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
        public double Longitude { get; set; }

        /// <summary>
        /// Angular latitude coordinate.
        /// </summary>
        public double Latitude { get; set; }
    }
}