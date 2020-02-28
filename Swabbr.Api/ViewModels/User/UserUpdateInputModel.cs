using Newtonsoft.Json;
using Swabbr.Api.ViewModels.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Swabbr.Api.ViewModels.User
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
        [Required(AllowEmptyStrings = false)]
        [JsonProperty("country")]
        public string Country { get; set; }

        /// <summary>
        /// Date of birth for the given user.
        /// </summary>
        [Required]
        //[RegularExpression(@"^\d{2}-\d{2}-\d{4}$", ErrorMessage = "Birthdate must be in format dd-mm-yyyy")]
        [JsonProperty("birthDate")]
        public DateTime BirthDate { get; set; }

        /// <summary>
        /// The specified timezone of the user
        /// </summary>
        [JsonProperty("timezone")]
        //[RegularExpression(@"^UTC(\+|\-)\d{2}:\d{2}$", ErrorMessage = "Timezone must be in format UTC+xx:xx")]
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

        // TODO Don't put this here
        /// <summary>
        /// Phone number of the user stored as text.
        /// </summary>
        //[Required(AllowEmptyStrings = false)]
        //[RegularExpression(@"^\+\d{11}$", ErrorMessage = "Phone number must be in format +xxxxxxxxxxxxx")]
        //[JsonProperty("phoneNumber")]
        //public string PhoneNumber { get; set; }

    }

}
