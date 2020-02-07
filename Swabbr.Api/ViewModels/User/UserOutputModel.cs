using Newtonsoft.Json;
using Swabbr.Core.Enums;
using System;

namespace Swabbr.Api.ViewModels
{
    public partial class UserOutputModel
    {
        /// <summary>
        /// Unique identifier.
        /// </summary>
        [JsonProperty("userId")]
        public Guid UserId { get; set; }

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
        /// Email address.
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; set; }

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
        /// Total amount of followers the user has acquired.
        /// </summary>
        [JsonProperty("totalFollowers")]
        public int TotalFollowers { get; set; }

        /// <summary>
        /// Total amount of users the subject is following.
        /// </summary>
        [JsonProperty("totalFollowing")]
        public int TotalFollowing { get; set; }

        /// <summary>
        /// Total amount of vlogs posted by the user.
        /// </summary>
        [JsonProperty("totalVlogs")]
        public int TotalVlogs { get; set; }

        /// <summary>
        /// Indicates whether the profile of the user is publicly visible to other users.
        /// </summary>
        [JsonProperty("isPrivate")]
        public bool IsPrivate { get; set; }
    }
}