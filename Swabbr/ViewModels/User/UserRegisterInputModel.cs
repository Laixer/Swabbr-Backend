using Newtonsoft.Json;
using Swabbr.Core.Entities;
using Swabbr.Core.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Swabbr.Api.ViewModels
{
    public class UserRegisterInputModel
    {
        /// <summary>
        /// First name of the user.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [JsonProperty("firstName", Required = Required.DisallowNull)]
        public string FirstName { get; set; }

        /// <summary>
        /// Surname of the user.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [JsonProperty("lastName", Required = Required.DisallowNull)]
        public string LastName { get; set; }

        /// <summary>
        /// Selected gender of the user.
        /// </summary>
        [Required]
        [JsonProperty("gender")]
        public Gender Gender { get; set; }

        /// <summary>
        /// Selected country.
        /// </summary>
        [Required]
        [JsonProperty("country")]
        public string Country { get; set; }

        /// <summary>
        /// Email address.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [JsonProperty("email", Required = Required.DisallowNull)]
        [EmailAddress, DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        /// <summary>
        /// Password input of the user.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [JsonProperty("password", Required = Required.DisallowNull)]
        public string Password { get; set; }

        /// <summary>
        /// Date of birth for the given user.
        /// </summary>
        [Required]
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
        [Required(AllowEmptyStrings = false)]
        [JsonProperty("nickname", Required = Required.DisallowNull)]
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
        /// Phone number of the user stored as text.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [JsonProperty("phoneNumber", Required = Required.DisallowNull)]
        public string PhoneNumber { get; set; }

        public static implicit operator User(UserRegisterInputModel user)
        {
            return new User
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                BirthDate = user.BirthDate,
                Email = user.Email,
                Country = user.Country,
                Gender = user.Gender,
                IsPrivate = user.IsPrivate,
                Nickname = user.Nickname,
                ProfileImageUrl = user.ProfileImageUrl,
                Timezone = user.Timezone,
                PasswordHash = user.Password,
                PhoneNumber = user.PhoneNumber
            };
        }
    }
}