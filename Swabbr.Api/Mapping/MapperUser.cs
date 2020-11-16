using Swabbr.Api.Parsing;
using Swabbr.Api.ViewModels;
using Swabbr.Api.ViewModels.Enums;
using Swabbr.Api.ViewModels.User;
using Swabbr.Core.Entities;
using Swabbr.Core.Extensions;
using System;

namespace Swabbr.Api.Mapping
{

    /// <summary>
    /// Contains mapping functionality for <see cref="SwabbrUser"/> entities.
    /// </summary>
    internal static class MapperUser
    {

        internal static UserWithStatsOutputModel Map(SwabbrUserWithStats input)
        {
            if (input == null) { throw new ArgumentNullException(nameof(input)); }
            return new UserWithStatsOutputModel
            {
                Id = input.Id,
                FirstName = input.FirstName,
                LastName = input.LastName,
                Email = input.Email,
                BirthDate = input.BirthDate,
                Country = input.Country,
                Gender = MapperEnum.Map(input.Gender)?.GetEnumMemberAttribute(),
                IsPrivate = input.IsPrivate,
                Nickname = input.Nickname,
                ProfileImageBase64Encoded = input.ProfileImageBase64Encoded,
                Timezone = input.Timezone?.ToString(),
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
                Gender = MapperEnum.Map(input.Gender)?.GetEnumMemberAttribute(),
                IsPrivate = input.IsPrivate,
                Nickname = input.Nickname,
                ProfileImageBase64Encoded = input.ProfileImageBase64Encoded,
                Timezone = input.Timezone?.ToString()
            };
        }

        /// <summary>
        ///     Extracts the user settings from an internal user
        ///     object into a dedicated settings DTO.
        /// </summary>
        /// <param name="user">The user to extract from.</param>
        /// <returns>The user settings DTO.</returns>
        internal static UserSettingsOutputModel MapToSettings(SwabbrUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return new UserSettingsOutputModel
            {
                DailyVlogRequestLimit = user.DailyVlogRequestLimit,
                FollowMode = MapperEnum.Map(user.FollowMode).GetEnumMemberAttribute(),
                IsPrivate = user.IsPrivate,
                UserId = user.Id
            };
        }

        /// <summary>
        ///     Extracts the user statistics from an internal user
        ///     object into a dedicated statistics DTO.
        /// </summary>
        /// <param name="userWithStats">The user to extract from.</param>
        /// <returns>The user statistics DTO.</returns>
        internal static UserStatisticsOutputModel MapToStatistics(SwabbrUserWithStats userWithStats)
        {
            if (userWithStats == null) { throw new ArgumentNullException(nameof(userWithStats)); }
            return new UserStatisticsOutputModel
            {
                UserId = userWithStats.Id,
                TotalFollowers = userWithStats.TotalFollowers,
                TotalFollowing = userWithStats.TotalFollowing,
                TotalLikes = userWithStats.TotalLikes,
                TotalReactionsGiven = userWithStats.TotalReactionsGiven,
                TotalReactionsReceived = userWithStats.TotalReactionsReceived,
                TotalViews = userWithStats.TotalViews,
                TotalVlogs = userWithStats.TotalVlogs
            };
        }
    }
}
