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
        /// <summary>
        ///     Checks if a user exists in our data store.
        /// </summary>
        /// <param name="userId">The user id.</param>
        Task<bool> ExistsAsync(Guid userId);

        /// <summary>
        ///     Checks if a nickname exists in our data store.
        /// </summary>
        /// <param name="nickname">The nickname to check.</param>
        Task<bool> ExistsNicknameAsync(string nickname);

        /// <summary>
        ///     Gets all users which are eligible for a vlog request.
        /// </summary>
        /// <returns>User collection.</returns>
        Task<IEnumerable<SwabbrUser>> GetAllVloggableUsersAsync();

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

        /// <summary>
        ///     Gets a user by an email.
        /// </summary>
        /// <param name="email">The user email.</param>
        /// <returns>The corresponding user.</returns>
        Task<SwabbrUser> GetByEmailAsync(string email);

        /// <summary>
        ///     Gets all followers for a user.
        /// </summary>
        /// <param name="userId">The user to check.</param>
        /// <returns>All followers.</returns>
        Task<IEnumerable<SwabbrUser>> GetFollowersAsync(Guid userId);

        /// <summary>
        ///     Gets all users a user is following.
        /// </summary>
        /// <param name="userId">The user to check.</param>
        /// <returns>All users followed by <paramref name="userId"/>.</returns>
        Task<IEnumerable<SwabbrUser>> GetFollowingAsync(Guid userId);

        /// <summary>
        ///     Gets the details required to send a push notification.
        /// </summary>
        /// <param name="userId">The user to check.</param>
        /// <returns>Push notification details.</returns>
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

        /// <summary>
        ///     Updates a user location in our data store.
        /// </summary>
        /// <param name="userId">The user to update.</param>
        /// <param name="longitude">New longitude coordinate.</param>
        /// <param name="latitude">New latitude coordinate.</param>
        Task UpdateLocationAsync(Guid userId, double longitude, double latitude);

        /// <summary>
        ///     Updates a user timezone in our data store.
        /// </summary>
        /// <param name="userId">The user to update.</param>
        /// <param name="newTimeZone">The new user timezone.</param>
        Task UpdateTimeZoneAsync(Guid userId, TimeZoneInfo newTimeZone);
    }
}
