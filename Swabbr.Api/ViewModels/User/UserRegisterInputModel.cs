using Newtonsoft.Json;
using Swabbr.Api.ViewModels.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Swabbr.Api.ViewModels.User
{

    /// <summary>
    /// Input model for registering a new user.
    /// </summary>
    public class UserRegisterInputModel
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
        public string Country { get; set; }

        /// <summary>
        /// Email address.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [EmailAddress, DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        /// <summary>
        /// Password input of the user.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public string Password { get; set; }

        /// <summary>
        /// Date of birth for the given user.
        /// </summary>
        //[RegularExpression(@"^\d{2}\-\d{2}\-\d{4}$", ErrorMessage = "Birthdate must be in format dd-mm-yyyy")]
        public DateTime? BirthDate { get; set; }

        /// <summary>
        /// Nickname to display for the user.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public string Nickname { get; set; }

        /// <summary>
        /// Base-64 encoded profile image string.
        /// </summary>
        public string ProfileImageBase64Encoded { get; set; }

        /// <summary>
        /// Indicates whether the profile of the user is publicly visible to other users.
        /// </summary>
        public bool? IsPrivate { get; set; }

        /// <summary>
        /// Phone number of the user stored as text.
        /// </summary>
        public string PhoneNumber { get; set; }

    }

}
