using Swabbr.Core.Entities;
using Swabbr.Core.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.Services
{
    /// <summary>
    ///     Contract for user related operations.
    /// </summary>
    /// <remarks>
    ///     The executing user id is never passed. Whenever possible,
    ///     this id is extracted from the context.
    /// </remarks>
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
        /// <param name="navigation">Navigation control.</param>
        /// <returns>User collection.</returns>
        IAsyncEnumerable<User> GetAllVloggableUsersAsync(Navigation navigation);

        /// <summary>
        ///     Gets a user from our data store.
        /// </summary>
        /// <param name="userId">The internal user id.</param>
        /// <returns>The user entity.</returns>
        Task<User> GetAsync(Guid userId);

        /// <summary>
        ///     Gets a user object with its statistics.
        /// </summary>
        /// <param name="userId">The internal user id.</param>
        /// <returns>The user entity with statistics.</returns>
        Task<UserWithStats> GetWithStatisticsAsync(Guid userId);

        /// <summary>
        ///     Gets all followers for a user.
        /// </summary>
        /// <param name="userId">The user to check.</param>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>All followers.</returns>
        IAsyncEnumerable<User> GetFollowersAsync(Guid userId, Navigation navigation);

        /// <summary>
        ///     Gets all users a user is following.
        /// </summary>
        /// <param name="userId">The user to check.</param>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>All users followed by <paramref name="userId"/>.</returns>
        IAsyncEnumerable<User> GetFollowingAsync(Guid userId, Navigation navigation);

        /// <summary>
        ///     Gets the details required to send a push notification.
        /// </summary>
        /// <param name="userId">The user to check.</param>
        /// <returns>Push notification details.</returns>
        Task<UserPushNotificationDetails> GetUserPushDetailsAsync(Guid userId);

        /// <summary>
        ///     Search for users in our data store.
        /// </summary>
        /// <param name="query">Search string.</param>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>User search result set.</returns>
        IAsyncEnumerable<UserWithRelationWrapper> SearchAsync(string query, Navigation navigation);

        /// <summary>
        ///     Update the current user in our data store.
        /// </summary>
        /// <remarks>
        ///     This can also be used to update user settings.
        /// </remarks>
        /// <param name="user">The user with updated properties.</param>
        Task UpdateAsync(User user);
    }
}
