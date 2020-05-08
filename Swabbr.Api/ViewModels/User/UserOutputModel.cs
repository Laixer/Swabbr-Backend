using Microsoft.OData.Edm;
using Newtonsoft.Json;
using Swabbr.Api.ViewModels.Enums;
using System;

namespace Swabbr.Api.ViewModels.User
{

    /// <summary>
    /// Reprents a single user.
    /// </summary>
    public partial class UserOutputModel
    {

        /// <summary>
        /// Unique identifier.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// First name of the user.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Surname of the user.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Email address.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Selected gender of the user.
        /// </summary>
        public string Gender { get; set; }

        /// <summary>
        /// Selected country.
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Date of birth for the given user.
        /// </summary>
        public DateTime? BirthDate { get; set; }

        /// <summary>
        /// The specified timezone of the user
        /// </summary>
        public string Timezone { get; set; }

        /// <summary>
        /// Nickname to display for the user.
        /// </summary>
        public string Nickname { get; set; }

        /// <summary>
        /// Base64 encoded string containing the uploaded profile image of the user.
        /// </summary>
        public string ProfileImageBase64Encoded { get; set; }

        /// <summary>
        /// Total amount of followers the user has acquired.
        /// </summary>
        public int TotalFollowers { get; set; }

        /// <summary>
        /// Total amount of users the subject is following.
        /// </summary>
        public int TotalFollowing { get; set; }

        /// <summary>
        /// Total amount of vlogs posted by the user.
        /// </summary>
        public int TotalVlogs { get; set; }

        /// <summary>
        /// Indicates whether the profile of the user is publicly visible to other users.
        /// </summary>
        public bool IsPrivate { get; set; }

    }

}
