using Newtonsoft.Json;
using Swabbr.Core.Entities;
using Swabbr.Core.Enums;
using System;
using System.Linq;

namespace Swabbr.Api.ViewModels
{
    public class UserOutputModel
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

        public static implicit operator UserOutputModel(User user)
        {
            return new UserOutputModel
            {
                UserId = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                BirthDate = user.BirthDate,
                Email = user.Email,
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