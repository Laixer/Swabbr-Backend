using Swabbr.Core.Entities;
using Swabbr.Core.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.Services
{

    /// <summary>
    /// Contract for <see cref="SwabbrUser"/> related operations.
    /// </summary>
    public interface IUserService
    {

        Task<bool> ExistsAsync(Guid userId);

        Task<bool> ExistsNicknameAsync(string nickname);

        Task<IEnumerable<SwabbrUserMinified>> GetAllVloggableUserMinifiedAsync();

        Task<SwabbrUser> GetAsync(Guid userId);

        Task<SwabbrUser> GetByEmailAsync(string email);

        Task<IEnumerable<SwabbrUser>> GetFollowersAsync(Guid userId);

        Task<IEnumerable<SwabbrUser>> GetFollowingAsync(Guid userId);

        Task<UserSettings> GetUserSettingsAsync(Guid userId);

        Task<UserStatistics> GetUserStatisticsAsync(Guid userId);

        Task<SwabbrUser> UpdateAsync(UserUpdateWrapper user);

        Task UpdateLocationAsync(Guid userId, double longitude, double latitude);

        Task UpdateSettingsAsync(UserSettings userSettings);

        Task UpdateTimeZoneAsync(Guid userId, TimeZoneInfo newTimeZone);

    }

}
