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
        public Guid UserId { get; set; }

        /// <summary>
        /// Username.
        /// </summary>
        public string Username { get; set; }

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
            => new UserOutputModel
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

        //TODO Remove
        #region temporary


        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static UserOutputModel NewRandomMock()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            return new UserOutputModel
            {
                UserId = Guid.NewGuid(),
                Email = $"{RandomString(3)}@{RandomString(4)}.{RandomString(3)}",
                FirstName = RandomString(2),
                LastName = RandomString(10),
                BirthDate = DateTime.Now,
                Nickname = RandomString(4)
            };
        }
        #endregion
    }
}