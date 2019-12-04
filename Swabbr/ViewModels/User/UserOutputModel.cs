using Swabbr.Core.Entities;
using Swabbr.Core.Enums;
using System;

namespace Swabbr.Api.ViewModels
{
    public class UserOutputModel
    {
        /// <summary>
        /// Unique identifier.
        /// </summary>
        public Guid UserId { get; set; }

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
        public Gender Gender { get; set; }

        /// <summary>
        /// Selected country.
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Email address.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Date of birth for the given user.
        /// </summary>
        public DateTime BirthDate { get; set; }

        /// <summary>
        /// The specified timezone of the user
        /// </summary>
        public string Timezone { get; set; }

        /// <summary>
        /// Nickname to display for the user.
        /// </summary>
        public string Nickname { get; set; }

        /// <summary>
        /// URL containing the uploaded profile image of the user.
        /// </summary>
        public string ProfileImageUrl { get; set; }

        /// <summary>
        /// Indicates whether the profile of the user is publicly visible to other users.
        /// </summary>
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