using Laixer.Utility.Extensions;
using Swabbr.Api.Parsing;
using Swabbr.Api.ViewModels;
using Swabbr.Api.ViewModels.User;
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
                Timezone = input.Timezone.ToString(),
                TotalFollowers = input.TotalFollowers,
                TotalFollowing = input.TotalFollowing,
                TotalVlogs = input.TotalVlogs
            };
        }

        internal static UserOutputModel Map(SwabbrUser input)
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
                Timezone = input.Timezone.ToString()
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
                ProfileImageUrl = input.ProfileImageUrl
            };
        }

        internal static UserSettingsOutputModel Map(UserSettings settings)
        {
            if (settings == null) { throw new ArgumentNullException(nameof(settings)); }
            return new UserSettingsOutputModel
            {
                DailyVlogRequestLimit = settings.DailyVlogRequestLimit,
                FollowMode = MapperEnum.Map(settings.FollowMode).GetEnumMemberAttribute(),
                IsPrivate = settings.IsPrivate,
                UserId = settings.UserId
            };
        }

        internal static UserStatisticsOutputModel Map(UserStatistics userStatistics)
        {
            if (userStatistics == null) { throw new ArgumentNullException(nameof(userStatistics)); }
            return new UserStatisticsOutputModel
            {
                TotalFollowers = userStatistics.TotalFollowers,
                TotalFollowing = userStatistics.TotalFollowing,
                TotalLikes = userStatistics.TotalLikes,
                TotalReactionsGiven = userStatistics.TotalReactionsGiven,
                TotalReactionsReceived = userStatistics.TotalReactionsReceived,
                TotalViews = userStatistics.TotalViews,
                TotalVlogs = userStatistics.TotalVlogs
            };
        }

    }

}
