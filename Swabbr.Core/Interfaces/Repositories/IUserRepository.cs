using Swabbr.Core.Entities;
using Swabbr.Core.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.Repositories
{

    /// <summary>
    /// Repository for <see cref="SwabbrUser"/> entities.
    /// </summary>
    public interface IUserRepository : IRepository<SwabbrUser, Guid>, ICudFunctionality<SwabbrUser, Guid>
    {

        Task<UserSettings> GetUserSettingsAsync(Guid userId);

        Task<UserStatistics> GetUserStatisticsAsync(Guid userId);

        // TODO Do we need this?
        Task UpdateUserSettingsAsync(UserSettings userSettings);

        Task<SwabbrUser> GetByEmailAsync(string email);

        Task<bool> UserExistsAsync(Guid userId);

        // TODO Move to withstats
        Task<IEnumerable<SwabbrUser>> GetFollowingAsync(Guid userId);

        // TODO Move to withstats
        Task<IEnumerable<SwabbrUser>> GetFollowersAsync(Guid userId);

        Task<IEnumerable<UserPushNotificationDetails>> GetFollowersPushDetailsAsync(Guid userId);

        Task<UserPushNotificationDetails> GetPushDetailsAsync(Guid userId);

        Task<IEnumerable<SwabbrUser>> GetVlogRequestableUsersAsync(DateTimeOffset from, TimeSpan timeSpan);

        Task<IEnumerable<SwabbrUserMinified>> GetAllVloggableUserMinifiedAsync();

        Task<SwabbrUser> GetUserFromVlogAsync(Guid vlogId);

        Task UpdateLocationAsync(Guid userId, double longitude, double latitude);

        Task UpdateTimeZoneAsync(Guid userId, TimeZoneInfo newTimeZone);

    }

}
