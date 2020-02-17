using Swabbr.Api.ViewModels;
using Swabbr.Core.Entities;
using System;

namespace Swabbr.Api.Mapping
{

    /// <summary>
    /// Contains mapping functionality for <see cref="SwabbrUser"/> entities.
    /// </summary>
    internal static class MapperUser
    {

        internal static UserOutputModel Map(SwabbrUserWithStats input)
        {
            if (input == null) { throw new ArgumentNullException(nameof(input)); }
            return new UserOutputModel
            {
                Id = input.Id,
                FirstName = input.FirstName,
                LastName = input.LastName,
                Email = input.Email,
                BirthDate = input.BirthDate,
                Country = input.Country,
                Gender = input.Gender,
                IsPrivate = input.IsPrivate,
                Nickname = input.Nickname,
                ProfileImageUrl = (input.ProfileImageUrl == null) ? "" : input.ProfileImageUrl.ToString(), // TODO Double check this
                Timezone = input.Timezone,
                TotalFollowers = input.TotalFollowers,
                TotalFollowing = input.TotalFollowing,
                TotalVlogs = input.TotalVlogs
            };
        }

        internal static SwabbrUser Map(UserUpdateInputModel input)
        {
            throw new NotImplementedException();
        }
    }

}
