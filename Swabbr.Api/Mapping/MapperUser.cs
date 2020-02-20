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
                Gender = MapperEnum.Map(input.Gender),
                IsPrivate = input.IsPrivate,
                Nickname = input.Nickname,
                ProfileImageUrl = input.ProfileImageUrl,
                Timezone = input.Timezone,
                TotalFollowers = input.TotalFollowers,
                TotalFollowing = input.TotalFollowing,
                TotalVlogs = input.TotalVlogs
            };
        }

        internal static SwabbrUser Map(UserUpdateInputModel input)
        {
            if (input == null) { throw new ArgumentNullException(nameof(input)); }
            return new SwabbrUser
            {
                BirthDate = input.BirthDate,
                Country = input.Country,
                //DailyVlogRequestLimit = ??
                //Email = ??,
                FirstName = input.FirstName,
                //FollowMode = ??
                Gender = MapperEnum.Map(input.Gender),
                LastName = input.LastName,
                IsPrivate = input.IsPrivate,
                //Latitude = ??
                //Longitude = ??
                Nickname = input.Nickname,
                ProfileImageUrl = input.ProfileImageUrl,
                Timezone = input.Timezone
            };
        }

        internal static UserSettingsOutputModel Map(UserSettings settings)
        {
            if (settings == null) { throw new ArgumentNullException(nameof(settings)); }
            return new UserSettingsOutputModel
            {
                DailyVlogRequestLimit = settings.DailyVlogRequestLimit,
                FollowMode = MapperEnum.Map(settings.FollowMode),
                IsPrivate = settings.IsPrivate,
                UserId = settings.UserId
            };
        }

    }

}
