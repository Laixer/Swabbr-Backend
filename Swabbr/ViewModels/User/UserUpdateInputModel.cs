using Newtonsoft.Json;
using Swabbr.Core.Entities;
using Swabbr.Core.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Swabbr.Api.ViewModels
{
    public class UserUpdateInputModel
    {
        /// <summary>
        /// First name of the user.
        /// </summary>
        [Required, JsonProperty("firstName")]
        public string FirstName { get; set; }

        /// <summary>
        /// Surname of the user.
        /// </summary>
        [Required, JsonProperty("lastName")]
        public string LastName { get; set; }

        /// <summary>
        /// Selected gender of the user.
        /// </summary>
        [Required, JsonProperty("gender")]
        public Gender Gender { get; set; }

        /// <summary>
        /// Selected country.
        /// </summary>
        [Required, JsonProperty("country")]
        public string Country { get; set; }

        /// <summary>
        /// Date of birth for the given user.
        /// </summary>
        [Required, JsonProperty("birthDate")]
        public DateTime BirthDate { get; set; }

        /// <summary>
        /// The specified timezone of the user
        /// </summary>
        [JsonProperty("timezone")]
        public string Timezone { get; set; }

        /// <summary>
        /// Nickname to display for the user.
        /// </summary>
        [Required, JsonProperty("nickname")]
        public string Nickname { get; set; }

        /// <summary>
        /// URL containing the uploaded profile image of the user.
        /// </summary>
        [JsonProperty("profileImageUrl")]
        public string ProfileImageUrl { get; set; }

        // TODO Get back to this, should this be in the user model or settings? It is related to the user profile.
        /// <summary>
        /// Indicates whether the profile of the user is publicly visible to other users.
        /// </summary>
        [JsonProperty("isPrivate")]
        public bool IsPrivate { get; set; }

        /// <summary>
        /// Phone number of the user stored as text.
        /// </summary>
        [Required, JsonProperty("phoneNumber")]
        public string PhoneNumber { get; set; }

        public static implicit operator User(UserUpdateInputModel user)
            => new User
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                BirthDate = user.BirthDate,
                Country = user.Country,
                Gender = user.Gender,
                IsPrivate = user.IsPrivate,
                Nickname = user.Nickname,
                ProfileImageUrl = user.ProfileImageUrl,
                Timezone = user.Timezone,
                PhoneNumber = user.PhoneNumber
            };
    }
}