using Newtonsoft.Json;
using Swabbr.Api.ViewModels.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Swabbr.Api.ViewModels
{

    /// <summary>
    /// Input model for updating a user entity.
    /// </summary>
    public class UserUpdateInputModel
    {

        /// <summary>
        /// First name of the user.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        /// <summary>
        /// Surname of the user.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [JsonProperty("lastName")]
        public string LastName { get; set; }

        /// <summary>
        /// Selected gender of the user.
        /// </summary>
        [Required]
        [JsonProperty("gender")]
        public GenderModel Gender { get; set; }

        /// <summary>
        /// Selected country.
        /// </summary>
        [Required]
        [JsonProperty("country")]
        public string Country { get; set; }

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
        /// Phone number of the user stored as text.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [JsonProperty("phoneNumber")]
        public string PhoneNumber { get; set; }

    }

}
