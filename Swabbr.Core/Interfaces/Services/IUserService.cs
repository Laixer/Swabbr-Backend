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

        /// <summary>
        ///     Gets a user from our data store.
        /// </summary>
        /// <param name="userId">The internal user id.</param>
        /// <returns>The user entity.</returns>
        Task<SwabbrUser> GetAsync(Guid userId);

        /// <summary>
        ///     Gets a user object with its statistics.
        /// </summary>
        /// <param name="userId">The internal user id.</param>
        /// <returns>The user entity with statistics.</returns>
        Task<SwabbrUserWithStats> GetWithStatisticsAsync(Guid userId);

        Task<SwabbrUser> GetByEmailAsync(string email);

        Task<IEnumerable<SwabbrUser>> GetFollowersAsync(Guid userId);

        Task<IEnumerable<SwabbrUser>> GetFollowingAsync(Guid userId);

        Task<UserPushNotificationDetails> GetUserPushDetailsAsync(Guid userId);

        /// <summary>
        ///     Update a user in our data store.
        /// </summary>
        /// <remarks>
        ///     This can also be used to update user settings.
        /// </remarks>
        /// <param name="user">The user with updated properties.</param>
        /// <returns>The post-updated user entity.</returns>
        Task<SwabbrUser> UpdateAsync(SwabbrUser user);

        Task UpdateLocationAsync(Guid userId, double longitude, double latitude);

        Task UpdateTimeZoneAsync(Guid userId, TimeZoneInfo newTimeZone);
    }
}
