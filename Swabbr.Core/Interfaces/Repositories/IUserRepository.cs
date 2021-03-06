﻿using Swabbr.Core.Entities;
using Swabbr.Core.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swabbr.Core.Interfaces.Repositories
{
    /// <summary>
    ///     Repository for a user repository.
    /// </summary>
    public interface IUserRepository : IRepository<User, Guid>,
        IUpdateRepository<User, Guid>
    {
        /// <summary>
        ///     Checks if a nickname already exists.
        /// </summary>
        /// <param name="nickname">The nickname to check for.</param>
        Task<bool> ExistsNicknameAsync(string nickname);

        /// <summary>
        ///     Gets a user with its statistics.
        /// </summary>
        /// <param name="userId">The internal user id.</param>
        /// <returns>The user entity with statistics.</returns>
        Task<UserWithStats> GetWithStatisticsAsync(Guid userId);

        /// <summary>
        ///     Gets users that are being followed by a given user.
        /// </summary>
        /// <param name="userId">The user that is following.</param>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>Users that the user is following.</returns>
        IAsyncEnumerable<User> GetFollowingAsync(Guid userId, Navigation navigation);

        /// <summary>
        ///     Gets the followers of a given user.
        /// </summary>
        /// <param name="userId">The user that is being followed.</param>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>Followers of the specified user.</returns>
        IAsyncEnumerable<User> GetFollowersAsync(Guid userId, Navigation navigation);

        /// <summary>
        ///     Gets the push notification details of the 
        ///     followers for a given user.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>Followers push notification details.</returns>
        IAsyncEnumerable<UserPushNotificationDetails> GetFollowersPushDetailsAsync(Guid userId, Navigation navigation);

        /// <summary>
        ///     Gets all <see cref="UserWithRelationWrapper"/> objects that 
        ///     belong to the users which have pending follow requests for 
        ///     the current user.
        /// </summary>
        /// <param name="navigation">Result set control.</param>
        /// <returns>Wrappers around all users that liked saids vlogs.</returns>
        IAsyncEnumerable<UserWithRelationWrapper> GetFollowRequestingUsersAsync(Navigation navigation);

        /// <summary>
        ///     Gets the push notification details for a
        ///     given user.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns>Push notification details.</returns>
        Task<UserPushNotificationDetails> GetPushDetailsAsync(Guid userId);

        /// <summary>
        ///     Gets a collection of all users that are eligible
        ///     for a vlog request.
        /// </summary>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>Vloggable users.</returns>
        IAsyncEnumerable<User> GetAllVloggableUsersAsync(Navigation navigation);

        /// <summary>
        ///     Gets all <see cref="VlogLikingUserWrapper"/> objects that 
        ///     belong to the vlogs of a given <paramref name="userId"/>.
        /// </summary>
        /// <param name="navigation">Result set control.</param>
        /// <returns>Wrappers around all users that liked saids vlogs.</returns>
        IAsyncEnumerable<VlogLikingUserWrapper> GetVlogLikingUsersForUserAsync(Navigation navigation);

        /// <summary>
        ///     Search for users in our data store.
        /// </summary>
        /// <param name="query">Search string.</param>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>User search result set.</returns>
        IAsyncEnumerable<UserWithRelationWrapper> SearchAsync(string query, Navigation navigation);
    }
}
