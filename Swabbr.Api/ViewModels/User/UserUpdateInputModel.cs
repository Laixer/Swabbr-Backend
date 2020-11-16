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
        public string FirstName { get; set; }

        /// <summary>
        /// Surname of the user.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Selected gender of the user.
        /// </summary>
        public GenderModel? Gender { get; set; }

        /// <summary>
        /// Selected country.
        /// </summary>
        [RegularExpression(@"^[A-Z]{3}$", ErrorMessage = "Country must be in ISO 3166-1 alpha-3 standard")]
        public string Country { get; set; }

        /// <summary>
        /// Date of birth for the given user.
        /// </summary>
        //[RegularExpression(@"^\d{2}\-\d{2}\-\d{4}$", ErrorMessage = "Birthdate must be in format dd-mm-yyyy")]
        public DateTime? BirthDate { get; set; }

        /// <summary>
        /// Nickname to display for the user.
        /// </summary>
        public string Nickname { get; set; }

        /// <summary>
        /// Base64 encoded string containing the uploaded profile image of the user.
        /// </summary>
        public string ProfileImageBase64Encoded { get; set; }

        // TODO Made non-nullable, is this correct?
        /// <summary>
        /// Indicates whether the profile of the user is publicly visible to other users.
        /// </summary>
        public bool IsPrivate { get; set; }

    }

}
