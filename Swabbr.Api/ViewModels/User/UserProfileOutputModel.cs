using Newtonsoft.Json;
using Swabbr.Core.Entities;
using Swabbr.Core.Enums;
using System;

namespace Swabbr.Api.ViewModels
{
    public class UserProfileOutputModel
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
        public Uri ProfileImageUrl { get; set; }

        /// <summary>
        /// Indicates whether the profile of the user is publicly visible to other users.
        /// </summary>
        [JsonProperty("isPrivate")]
        public bool IsPrivate { get; set; }

        /// <summary>
        /// Represents the total amount of recorded vlogs this user profile contains.
        /// </summary>
        [JsonProperty("totalVlogs")]
        public int TotalVlogs { get; set; }

        /// <summary>
        /// Represents the total amount of users following this user profile.
        /// </summary>
        [JsonProperty("totalFollowers")]
        public int TotalFollowers { get; set; }

        /// <summary>
        /// Represents the total amount of user profiles this user is following.
        /// </summary>
        [JsonProperty("totalFollowing")]
        public int TotalFollowing { get; set; }

        public static implicit operator UserProfileOutputModel(SwabbrUser user)
        {
            return new UserProfileOutputModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                BirthDate = user.BirthDate,
                Country = user.Country,
                Gender = user.Gender,
                IsPrivate = user.IsPrivate,
                Nickname = user.Nickname,
                ProfileImageUrl = user.ProfileImageUrl,
                Timezone = user.Timezone
            };
        }
    }
}