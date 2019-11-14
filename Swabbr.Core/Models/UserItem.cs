using Newtonsoft.Json;
using Swabbr.Core.Enums;
using System;

namespace Swabbr.Core.Models
{
    /// <summary>
    /// Represents a single user.
    /// </summary>
    public class UserItem : Entity
    {
        /// <summary>
        /// First name of the user.
        /// </summary>
        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        /// <summary>
        /// Surname of the user.
        /// </summary>
        [JsonProperty("lastName")]
        public string LastName { get; set; }

        /// <summary>
        /// Selected gender of the user.
        /// </summary>
        [JsonProperty("gender")]
        public Gender Gender { get; set; }

        /// <summary>
        /// Selected country.
        /// </summary>
        [JsonProperty("country")]
        public string Country { get; set; }

        /// <summary>
        /// Email address.
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; set; }

        /// <summary>
        /// Date of birth for the given user.
        /// </summary>
        [JsonProperty("birthDate")]
        public DateTime BirthDate { get; set; }

        /// <summary>
        /// The specified timezone of the user
        /// </summary>
        [JsonProperty("timezone")]
        public string Timezone { get; set; }

        /// <summary>
        /// Nickname to display for the user.
        /// </summary>
        [JsonProperty("nickname")]
        public string Nickname { get; set; }

        /// <summary>
        /// URL containing the uploaded profile image of the user.
        /// </summary>
        [JsonProperty("profileImageUrl")]
        public string ProfileImageUrl { get; set; }

        /// <summary>
        /// Indicates whether the profile of the user is publicly visible to other users.
        /// </summary>
        [JsonProperty("isPrivate")]
        public bool IsPrivate { get; set; }

        /// <summary>
        /// Angular longitude coordinate.
        /// </summary>
        [JsonProperty("longitude")]
        public decimal Longitude { get; set; }

        /// <summary>
        /// Angular latitude coordinate.
        /// </summary>
        [JsonProperty("latitude")]
        public decimal Latitude { get; set; }

        /// <summary>
        /// Phone number of the user stored as text.
        /// </summary>
        [JsonProperty("phoneNumber")]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Personal settings and preferences.
        /// </summary>
        [JsonProperty("settings")]
        public Settings Settings { get; set; }
    }
}