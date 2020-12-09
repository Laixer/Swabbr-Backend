﻿using Swabbr.Core.Abstractions;
using Swabbr.Core.Entities;
using Swabbr.Core.Interfaces.Repositories;
using Swabbr.Core.Interfaces.Services;
using Swabbr.Core.Types;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swabbr.Core.Services
{
    /// <summary>
    ///     Service to handle user related operations.
    /// </summary>
    public class UserService :  AppServiceBase, IUserService
    {
        protected readonly IUserRepository _userRepository;

        /// <summary>
        ///     Create new instance.
        /// </summary>
        public UserService(AppContext appContext,
            IUserRepository userRepository)
        {
            AppContext = appContext ?? throw new ArgumentNullException(nameof(appContext));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        /// <summary>
        ///     Checks if a user exists in our data store.
        /// </summary>
        /// <param name="userId">The user id.</param>
        public virtual Task<bool> ExistsAsync(Guid userId) 
            => _userRepository.ExistsAsync(userId);

        /// <summary>
        ///     Checks if a nickname exists in our data store.
        /// </summary>
        /// <param name="nickname">The nickname to check.</param>
        public virtual Task<bool> ExistsNicknameAsync(string nickname) 
            => _userRepository.ExistsNicknameAsync(nickname);

        /// <summary>
        ///     Gets all users which are eligible for a vlog request.
        /// </summary>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>Vloggable user collection</returns>
        public virtual IAsyncEnumerable<User> GetAllVloggableUsersAsync(Navigation navigation) 
            => _userRepository.GetAllVloggableUsersAsync(navigation);

        /// <summary>
        ///     Get a single user from our data store.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <returns>The user.</returns>
        public virtual Task<User> GetAsync(Guid userId) 
            => _userRepository.GetAsync(userId);

        /// <summary>
        ///     Gets all followers for a user.
        /// </summary>
        /// <param name="userId">The user to check.</param>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>All followers.</returns>
        public virtual IAsyncEnumerable<User> GetFollowersAsync(Guid userId, Navigation navigation)
            => _userRepository.GetFollowersAsync(userId, navigation);

        /// <summary>
        ///     Gets all users a user is following.
        /// </summary>
        /// <param name="userId">The user to check.</param>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>All users followed by <paramref name="userId"/>.</returns>
        public virtual IAsyncEnumerable<User> GetFollowingAsync(Guid userId, Navigation navigation)
            => _userRepository.GetFollowingAsync(userId, navigation);

        /// <summary>
        ///     Gets the details required to send a push notification.
        /// </summary>
        /// <param name="userId">The user to check.</param>
        /// <returns>Push notification details.</returns>
        public virtual Task<UserPushNotificationDetails> GetUserPushDetailsAsync(Guid userId) 
            => _userRepository.GetPushDetailsAsync(userId);

        /// <summary>
        ///     Gets a user with corresponding statistics.
        /// </summary>
        /// <param name="userId">The internal user id.</param>
        /// <returns>User entity with statistics.</returns>
        public virtual Task<UserWithStats> GetWithStatisticsAsync(Guid userId)
            => _userRepository.GetWithStatisticsAsync(userId);

        /// <summary>
        ///     Search for users in our data store.
        /// </summary>
        /// <param name="query">Search string.</param>
        /// <param name="navigation">Navigation control.</param>
        /// <returns>User search result set.</returns>
        public virtual IAsyncEnumerable<User> SearchAsync(string query, Navigation navigation)
            => _userRepository.SearchAsync(query, navigation);

        /// <summary>
        ///     This updates the current user in our database.
        /// </summary>
        /// <param name="user">The updated user entity.</param>
        /// <returns>The user entity after the update operation.</returns>
        public virtual Task UpdateAsync(User user)
            => _userRepository.UpdateAsync(user);
    }
}
